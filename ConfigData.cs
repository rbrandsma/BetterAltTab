namespace BetterAltTab;

internal class ConfigDataBase
{
    internal static float MinVersion = 0.5f;
    internal static float MaxVersion = 1f;
    internal virtual float ConfigVersion { get; set; } = .5f;
}

internal class ConfigDataV1 : ConfigDataBase
{
    internal TabSwitcherDataV1 TabSwitcherData { get; set; } = new TabSwitcherDataV1();
}

internal class TabSwitcherDataBase
{
    internal virtual float ConfigVersion { get; set; } = .5f;
}

internal class TabSwitcherDataV1 : TabSwitcherDataBase
{
    internal override float ConfigVersion { get; set; } = 1f;
    internal string? BackgroundImagePath { get; set; } = null;
    internal bool StartVisable { get; set; } = false;
    internal FormBorderStyle BorderStyle { get; set; } = FormBorderStyle.None;
    internal FormStartPosition StartPosition { get; set; } = FormStartPosition.CenterScreen;
    internal Point? Location { get; set; } = null;
    internal Rectangle? WindowSize { get; set; } = null;
}

internal static class TabSwitcherDataConverter
{
    internal static TabSwitcherDataV1 BaseToV1(TabSwitcherDataBase data)
    {
        return new TabSwitcherDataV1();
    }
}
