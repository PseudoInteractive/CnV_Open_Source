﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;



namespace CnVDiscord
{
    class Bridge
    {
        public static async void OnBC(ServerBroadcastEventArgs args)
        {
            try
            {
                string msg = args.Message.ToString();
                var chat = await Discord.DiscordBot.GetChannelAsync(Discord.Config.ChatID);

                try
                {
                    Regex regex = new Regex(@"\[i:(\S*)]");
                    MatchCollection matches = regex.Matches(msg);

                    if (matches.Count > 0)
                        msg = regex.Replace(msg, "");

                    Regex isPlayer = new Regex(@".*:");
                    Regex regex1 = new Regex(@".* was .*kicked for '.*'");
                    Regex regex2 = new Regex(@".* .*kicked .* for '.*'");

                    if (msg.Contains("(Server Broadcast)") || regex1.IsMatch(msg) || regex2.IsMatch(msg))
                    {
                        await Discord.DiscordBot.SendMessageAsync(chat, $"*{msg}*");
                    }
                }
                catch (Exception a)
                {
                    COTG.Debug.LogEx(a,false,$"DiscordBridge error when sending message to discord: {a.Message}");
                }
            }
            catch (RateLimitException ex) { }

            return;
        }
        public static async void SendMessage(string message)
        {
           
        }

        //public static void OnKill(object sender, GetDataHandlers.KillMeEventArgs args)
        //{
        //    SendMessage("*" + args.PlayerDeathReason.GetDeathText(args.Player.Name).ToString() + "*"); //.GetDeathText(args.Player.Name).ToString()

        //    IEnumerable<TSPlayer> alive = from ply in TShock.Players
        //                                  where !ply.Dead && ply != null && ply.Name != ""
        //                                  select ply;
        //    if (TShock.Utils.GetActivePlayerCount() > 0 && Plugin4PDA.Bosses.Count() > 0)
        //    {
        //        if (!alive.Any())
        //        {
        //            string bosses = "";
        //            foreach (Plugin4PDA.BossFight boss in Plugin4PDA.Bosses)
        //            {
        //                if (bosses == "")
        //                    bosses = boss.Boss.FullName;
        //                else
        //                    bosses += ", " + boss.Boss.FullName;
        //            }

        //            Plugin4PDA.Bosses.Clear();

        //            TSPlayer.All.SendErrorMessage("Последний игрок(" + args.Player.Name + ") погиб и босс(ы) " + bosses + " улетел(и)!");
        //            SendMessage(null, "***Последний игрок (" + args.Player.Name + ") погиб и босс(ы) __" + bosses + "__ улетел(и)!***"); //.GetDeathText(args.Player.Name).ToString()
        //        }
        //    }
        //}

        //public static async void OnSaveWorld(WorldSaveEventArgs args)
        //{
        //    if (TShock.Config.AnnounceSave)
        //    {
        //        // Protect against internal errors causing save failures
        //        // These can be caused by an unexpected error such as a bad or out of date plugin
        //        try
        //        {
        //            var ch = await Discord.DiscordBot.GetChannelAsync(Discord.Config.ChatID);
        //            await Discord.DiscordBot.SendMessageAsync(ch, "*Saving world...*");
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.logger.Error("World saved notification failed");
        //            Log.logger.Error(ex.ToString());
        //        }
        //    }
        //}

        //public static async Task AutoBC()
        //{
        //    var chat = await Discord.DiscordBot.GetChannelAsync(Discord.Config.ChatID);
        //    Warn(chat);
        //}
        //private static async Task Warn(DiscordChannel chat)
        //{
        //    await Discord.DiscordBot.SendMessageAsync(chat, "**ВНИМАНИЕ!** Этот чат предназначен для общения непосредственно с игроками в игре. Всё остальное общение переносим в другие чаты!\n**ATTENTION!** This chat is for communicate directly with the players in the game. All other communication is transferred to other chats!");
        //    while (true)
        //    {
        //        if (TShock.Utils.GetActivePlayerCount() > 3)
        //            await Discord.DiscordBot.SendMessageAsync(chat, "**ВНИМАНИЕ!** Этот чат предназначен для общения непосредственно с игроками в игре. Всё остальное общение переносим в другие чаты!\n**ATTENTION!** This chat is for communicate directly with the players in the game. All other communication is transferred to other chats!");
        //        await Task.Delay(1200000);
        //    }
        //}
    }
}
