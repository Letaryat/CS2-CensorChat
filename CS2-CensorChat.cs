using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Core.Translations;
using System.Text.Json.Serialization;

namespace cs2_censorchat;
public class Censorconfig : BasePluginConfig
{
    [JsonPropertyName("Flag")] public string OmitFlag { get; set; } = "@chosen/one";

    [JsonPropertyName("Blocked words")] public string[] BlockedWords { get; set; } = ["word1", "word2"];
}

public class cs2_censorchat : BasePlugin, IPluginConfig<Censorconfig>
{
    public override string ModuleName => "CS2-CensorChat";

    public override string ModuleVersion => "0.0.1";

    public override string ModuleAuthor => "Letaryat";
    public override string ModuleDescription => "Block bad words :<";
    public Censorconfig Config { get; set; }    

    public void OnConfigParsed(Censorconfig config)
    {
        Config = config;
    }
    public override void Load(bool hotReload)
    {
        Console.WriteLine("CS2-CensorChat on!");
        AddCommandListener("say", OnPlayerChat);
        AddCommandListener("say_team", OnPlayerChat);
    }
    public override void Unload(bool hotReload)
    {
        Console.WriteLine("CS2-CensorChat off!");
    }

    public HookResult OnPlayerChat(CCSPlayerController? player, CommandInfo message)
    {
        if(AdminManager.PlayerHasPermissions(player, Config!.OmitFlag) || player == null || player.IsBot || player.IsHLTV) return HookResult.Continue;
        var msg = message.GetArg(1).ToLower();
        var word = Config.BlockedWords.FirstOrDefault(word => msg.Contains(word.ToLower()));
        if(Config.BlockedWords.Any(word => msg.Contains(word.ToLower())))
        {
            player.PrintToChat($"{Localizer["Prefix"]}{Localizer["Message", word!]}");
            return HookResult.Handled;
        }
        return HookResult.Continue;
    }
}
