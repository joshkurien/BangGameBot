﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BangGameBot
{
    public static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1) {
                byte[] box = new byte[1];
                do
                    provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static Card ChooseCardFromHand(this Player p){
            return p.CardsInHand.Random();
        }

        public static int DistanceSeen(this Player source, Player target, List<Player> players) {
            //TODO deal with dead players!
            var i = players.IndexOf(source);
            var j = players.IndexOf(target);
            //direct distance
            var dist1 = Math.Abs(j-i);
            //cycling distance
            var dist2 = players.Count()-Math.Max(i,j)+Math.Min(i,j);
            var distance = Math.Min(dist1, dist2);
            //account characters & cards!
            if (target.Character == Character.PaulRegret)
                distance++;
            if (target.CardsOnTable.Any(x => x.Name == CardName.Mustang))
                distance++;
            if (source.Character == Character.RoseDoolan)
                distance--;
            if (source.CardsOnTable.Any(x => x.Name == CardName.Scope))
                distance--;
            return distance < 1 ? 1 : distance;
        }


        public static string GetString<T>(this object en)
        {
            var text = Enum.GetName(typeof(T), en);
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static string ToBold(this string str)
        {
            return $"<b>{str.FormatHTML()}</b>";
        }

        public static string ToItalic(this string str)
        {
            return $"<i>{str.FormatHTML()}</i>";
        }

        public static string FormatHTML(this string str)
        {
            return str?.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        public static string ToEmoji(this int i)
        {
            var emojis = new [] { "1️⃣", "2️⃣", "3️⃣", "4️⃣", "5️⃣", "6️⃣", "7️⃣", "8️⃣", "9️⃣", "🔟" };
            return emojis[i-1];
        }

        public static string ToEmoji(this CardSuit s)
        {
            switch (s) {
                case CardSuit.Clubs:
                    return "♣️";
                case CardSuit.Diamonds:
                    return "♦️";
                case CardSuit.Hearts:
                    return "♥️";
                case CardSuit.Spades:
                    return "♠️";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string LivesString(this Player p)
        {
            string r = "";
            for (var i = 0; i < p.Lives; i++) {
                r += "❤️";
            }
            if (r == "")
                r = "💀 - " + p.Role.GetString<Role>();
            return r;
        }

        public static string GetDescription(this Card c) {
            var numberstring = "";
            switch (c.Number) {
                case 11:
                    numberstring = "J";
                    break;
                case 12:
                    numberstring = "Q";
                    break;
                case 13:
                    numberstring = "K";
                    break;
                case 14:
                    numberstring = "A";
                    break;
                default:
                    numberstring = c.Number.ToString();
                    break;
            }
            return c.Name.GetString<CardName>() + "[" + numberstring + c.Suit.ToEmoji() + "]";
        }

        public static string Encode(this Card c) {
            return ((int)c.Name).ToString() + "," + c.Number + "," + ((int)c.Suit).ToString() + "," + (c.IsOnTable ? "1" : "0");
        }

        public static Card GetCard(this string str, Dealer d, List<Player> p) {
            var ints = str.Split(',');
            //parse the card
            var name = (CardName)int.Parse(ints[0]);
            var number = int.Parse(ints[1]);
            var suit = (CardSuit)int.Parse(ints[2]);
            var isontable = ints[3] == "1";
            //now find where this card is
            return d.Deck.Union(p.SelectMany(x => x.Cards)).FirstOrDefault(x => x.Name == name && x.Number == number && x.Suit == suit && x.IsOnTable == isontable);
        }

        public static T[] ToSinglet<T>(this T obj) {
            return new [] { obj };
        }

        /// <summary>
        /// Returns a random item from the list.
        /// </summary>
        public static T Random<T>(this List<T> list) {
            return list.ElementAt(Program.R.Next(list.Count()));
        }


        //thanks Para! :P
        public static int ComputeLevenshtein(this string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // initialize the top and right of the table to 0, 1, 2, ...
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }

    }
}

