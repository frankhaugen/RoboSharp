using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using RoboSharp.World;

namespace RoboSharp.Studio.Shell;

/// <summary>Tile-based world preview for the sidebar — clearer than raw ASCII in a <see cref="TextBlock"/>.</summary>
public sealed class RobotWorldGridView : Border
{
    /// <summary>
    /// Logical cell size in DIPs for the grid inside the <see cref="Viewbox"/>.
    /// Star-sized rows/columns measure to zero without a definite constraint; fixed pixels give a real 2D extent so Uniform scaling works.
    /// </summary>
    private const double LogicalCellSizeDip = 20;

    private readonly Viewbox _viewbox;
    private Grid? _cellGrid;
    private Border?[,] _cells = new Border[0, 0];
    private TextBlock?[,] _glyphs = new TextBlock[0, 0];
    private int _width;
    private int _height;

    public RobotWorldGridView()
    {
        Background = StudioVisual.WorldGridChromeBrush;
        BorderBrush = StudioVisual.BorderSubtleBrush;
        BorderThickness = new Thickness(1);
        CornerRadius = new CornerRadius(8);
        Padding = new Thickness(6);
        ClipToBounds = true;
        BoxShadow = StudioVisual.SoftPanelShadow;
        // StackPanel gives the child indefinite vertical space; without a definite size the Viewbox
        // measures the inner Grid at 0×0. Fixed DIP extent + min on this border keeps the grid visible.
        MinWidth = 232;
        MinHeight = 232;
        HorizontalAlignment = HorizontalAlignment.Stretch;

        _viewbox = new Viewbox
        {
            Stretch = Stretch.Uniform,
            Width = 220,
            Height = 220,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
        };

        Child = _viewbox;
    }

    public void Update(RobotWorldSnapshot snapshot)
    {
        if (snapshot.Width <= 0 || snapshot.Height <= 0)
            return;

        EnsureGrid(snapshot.Width, snapshot.Height);

        var terrain = new TerrainCellKind[snapshot.Width, snapshot.Height];
        foreach (var t in snapshot.Tiles)
        {
            if (t.X >= 0 && t.X < snapshot.Width && t.Y >= 0 && t.Y < snapshot.Height)
                terrain[t.X, t.Y] = t.Terrain;
        }

        for (var y = 0; y < snapshot.Height; y++)
        {
            for (var x = 0; x < snapshot.Width; x++)
            {
                var actor = PrimaryActorAt(snapshot.Actors, x, y);
                var cell = _cells[x, y]!;
                var glyph = _glyphs[x, y]!;

                if (actor is not null)
                {
                    cell.Background = StudioVisual.WorldGridActorCellBrush;
                    cell.BorderBrush = StudioVisual.AccentBrush;
                    cell.BorderThickness = new Thickness(1.25);
                    glyph.Text = DirectionGlyph(actor.Direction);
                    glyph.Foreground = StudioVisual.WorldGridActorGlyphBrush;
                }
                else
                {
                    cell.BorderThickness = new Thickness(0.5);
                    cell.BorderBrush = StudioVisual.WorldGridCellEdgeBrush;
                    glyph.Text = string.Empty;
                    cell.Background = terrain[x, y] switch
                    {
                        TerrainCellKind.Wall => StudioVisual.WorldGridWallBrush,
                        TerrainCellKind.Goal => StudioVisual.WorldGridGoalBrush,
                        _ => StudioVisual.WorldGridFloorBrush,
                    };
                }
            }
        }
    }

    private void EnsureGrid(int width, int height)
    {
        if (_cellGrid is not null && _width == width && _height == height)
            return;

        _width = width;
        _height = height;

        _cellGrid = new Grid();
        var cellLength = new GridLength(LogicalCellSizeDip, GridUnitType.Pixel);
        for (var c = 0; c < width; c++)
            _cellGrid.ColumnDefinitions.Add(new ColumnDefinition(cellLength));

        for (var r = 0; r < height; r++)
            _cellGrid.RowDefinitions.Add(new RowDefinition(cellLength));

        _cells = new Border[width, height];
        _glyphs = new TextBlock[width, height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var label = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontFamily = StudioVisual.CodeFontFamily,
                    FontSize = 11,
                    FontWeight = FontWeight.Bold,
                };

                var cell = new Border
                {
                    Child = label,
                    CornerRadius = new CornerRadius(2),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Margin = new Thickness(0.35),
                };

                Grid.SetColumn(cell, x);
                Grid.SetRow(cell, y);
                _cellGrid.Children.Add(cell);
                _cells[x, y] = cell;
                _glyphs[x, y] = label;
            }
        }

        _viewbox.Child = _cellGrid;
    }

    private static ActorSnapshot? PrimaryActorAt(IReadOnlyList<ActorSnapshot> actors, int x, int y)
    {
        ActorSnapshot? pick = null;
        foreach (var a in actors)
        {
            if (a.X != x || a.Y != y)
                continue;
            if (a.Id == 1)
                return a;
            pick ??= a;
        }

        return pick;
    }

    private static string DirectionGlyph(Direction d) =>
        d switch
        {
            Direction.North => "▲",
            Direction.East => "▶",
            Direction.South => "▼",
            Direction.West => "◀",
            _ => "●",
        };
}
