﻿using Discord;
using Discord.WebSocket;
using MEE7.Backend.Configuration;
using System.Compat.Web;
using System.Linq;
using TweetSharp;

namespace MEE7.Backend.HelperFunctions
{
    public static class DiscordExtensions
    {
        public static EmbedBuilder ToEmbed(this IMessage m)
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithColor(0, 128, 255);
            Embed.WithAuthor(m.Author);

            if (string.IsNullOrWhiteSpace(m.Content))
                Embed.WithTitle(m.Attachments.Select(x => x.Url).
                    Where(x => !x.EndsWith(".png") && !x.EndsWith(".jpg")).
                    Union(new string[] { "-" }).
                    Aggregate((x, y) => y == "-" ? x : x + " " + y));
            else if (m.Content.Length > 256)
                Embed.WithTitle(m.Content.Substring(0, 253) + "...");
            else if (m.Content.StartsWith("```"))
                Embed.WithTitle(m.Content.Trim('`').GetEverythingBetween("\n", ""));
            else
                Embed.WithTitle(m.Content);

            try
            {
                if (m.Attachments.Count > 0)
                    Embed.WithImageUrl(m.Attachments.ElementAt(0).Url);
            }
            catch { }
            return Embed;
        }
        public static ulong GetServerID(this IMessage m)
        {
            return Program.GetGuildFromChannel(m.Channel).Id;
        }
        public static string GetDisplayName(this SocketGuildUser u)
        {
            if (u.Nickname != null)
                return u.Nickname;
            return u.Username;
        }
        public static Discord.Color? GetDisplayColor(this SocketGuildUser u)
        {
            SocketRole[] r = u.Roles.Where(x => x.Color.RawValue != 0).ToArray();
            if (r.Length > 0)
                return new Discord.Color(r.MaxElement(x => x.Position).Color.RawValue);
            else
                return null;
        }
        public static EmbedBuilder AddFieldDirectly(this EmbedBuilder e, string Name, object Value, bool IsInline = false)
        {
            string text = Value.ToString();
            if (string.IsNullOrWhiteSpace(text))
                return e;
            if (text.Length < 1024)
                e.Fields.Add(new EmbedFieldBuilder() { Name = Name, Value = Value, IsInline = IsInline });
            else
            {
                int i;
                for (i = 1; text.Length >= 1015; i++)
                {
                    int cutIndex = text.AllIndexesOf("\n").Where(x => x <= 1020).Max();
                    if (cutIndex <= 0) return e;
                    e.Fields.Add(new EmbedFieldBuilder() { Name = $"{Name} {i}", Value = text.Substring(0, cutIndex), IsInline = IsInline });
                    text = text.Remove(0, cutIndex);
                }
                e.Fields.Add(new EmbedFieldBuilder() { Name = $"{Name} {i}", Value = text, IsInline = IsInline });
            }
            return e;
        }
        public static SelfmadeMessage EditContent(this SelfmadeMessage m, string newContent)
        {
            m.Content = newContent;
            return m;
        }
        public static string Print(this IEmote e)
        {
            if (e is Emote)
                return e.ToString();
            else
                return e.Name;
        }
        public static string Print(this DiscordEmote e)
        {
            return e.ToIEmote().Print();
        }

        public static string GetContent(this TwitterStatus s)
        {
            string Content = s.Text;
            Content = HttpUtility.HtmlDecode(Content);
            Content = Content.Split(' ').SkipWhile(x => x.StartsWith("@")).Combine(" ");
            Content = Content.Replace("\\\"", "\"").Replace("\n", " ");

            foreach (var v in s.Entities.Urls)
                Content = Content.Replace(v.Value, v.ExpandedValue);

            foreach (var v in s.Entities.Media)
                Content = Content.Replace(v.Url, "");

            Content = Content.Trim(' ');

            return Content;
        }
    }
}
