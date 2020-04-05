﻿using Discord.WebSocket;
using MEE7.Backend;
using MEE7.Backend.HelperFunctions;
using MEE7.Backend.HelperFunctions.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace MEE7.Commands
{
    public class Overwatch : Command
    {
        public Overwatch() : base("overwatch", "Prints todays overwatch arcade game modes", isExperimental: false, isHidden: false)
        {

        }

        public override void Execute(SocketMessage message)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var gameMode = GetGamemodeJson("https://overwatcharcade.today/api/gamemodes".GetHTMLfromURL());
            var today = GetTodayJson("https://overwatcharcade.today/api/today".GetHTMLfromURL())[0];
            DiscordNETWrapper.SendText($"Updated at {today.CreatedAt.UtcDateTime.ToLongDateString()}, " +
                $"{today.CreatedAt.UtcDateTime.ToShortTimeString()} by {today.ByUser.Battletag}", message.Channel).Wait();
            foreach (Tile t in today.Tiles())
                DiscordNETWrapper.SendEmbed(DiscordNETWrapper.CreateEmbedBuilder(t.Name, t.Label == null ? t.Players : $"{t.Players} - {t.Label}", 
                    gameMode.FirstOrDefault(x => x.Id == t.Id)?.Image.Normal.AbsoluteUri), message.Channel).Wait();
        }

        static TodayJsonRoot[] GetTodayJson(string json) => JsonConvert.DeserializeObject<TodayJsonRoot[]>(json);
        static GamemodeJsonRoot[] GetGamemodeJson(string json) => JsonConvert.DeserializeObject<GamemodeJsonRoot[]>(json);

        // Today API JSON Structure - generated by https://app.quicktype.io/#l=cs
        class TodayJsonRoot
        {
            [JsonProperty("created_at")]
            public DateTimeOffset CreatedAt { get; set; }

            [JsonProperty("by_user")]
            public ByUser ByUser { get; set; }

            [JsonProperty("tile_1")]
            public Tile Tile1 { get; set; }

            [JsonProperty("tile_2")]
            public Tile Tile2 { get; set; }

            [JsonProperty("tile_3")]
            public Tile Tile3 { get; set; }

            [JsonProperty("tile_4")]
            public Tile Tile4 { get; set; }

            [JsonProperty("tile_5")]
            public Tile Tile5 { get; set; }

            [JsonProperty("tile_6")]
            public Tile Tile6 { get; set; }

            [JsonProperty("tile_7")]
            public Tile Tile7 { get; set; }

            public Tile[] Tiles() => new Tile[] { Tile1, Tile2, Tile3, Tile4, Tile5, Tile6, Tile7 };
        }
        class ByUser
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("battletag")]
            public string Battletag { get; set; }

            [JsonProperty("avatar")]
            public Uri Avatar { get; set; }
        }
        class Tile
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("players")]
            public string Players { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("label")]
            public string Label { get; set; }
        }

        // Gamemode API JSON Structure - generated by https://app.quicktype.io/#l=cs
        class GamemodeJsonRoot
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("players")]
            public string Players { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("image")]
            public Image Image { get; set; }
        }
        class Image
        {
            [JsonProperty("large")]
            public Uri Large { get; set; }

            [JsonProperty("normal")]
            public Uri Normal { get; set; }
        }
    }
}