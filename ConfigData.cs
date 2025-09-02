namespace BetterAltTab;

using System.Text.Json;

internal class ConfigDataBase
{
    internal float ConfigVersion { get; set; } = .5f;

}

internal class TabSwitcherDataBase
{
    internal float ConfigVersion { get; set; } = .5f;
    internal string? BackgroundImagePath { get; set; } = null;
    internal bool StartVisable { get; set; } = false;
    internal FormBorderStyle BorderStyle { get; set; } = FormBorderStyle.None;
    internal FormStartPosition StartPosition { get; set; } = FormStartPosition.CenterScreen;
    internal Rectangle? WindowSize { get; set; } = null;
}

internal class ConfigDataV1 : ConfigDataBase
{
    internal TabSwitcherDataV1? TabSwitcherData { get; set; } = null;
}

internal class TabSwitcherDataV1 : TabSwitcherDataBase
{

}
