using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace StompyBlondie
{

    internal class PosTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if(value is string)
            {
                var strVal = (string) value;
                foreach(var c in new []{"<", ">", " "})
                    strVal = strVal.Replace(c, string.Empty);
                var parts = strVal.Split(',');
                var pos = new Pos();
                pos.X = float.Parse(parts[0]);
                pos.Y = float.Parse(parts[1]);
                pos.Z = float.Parse(parts[2]);
                return pos;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    [TypeConverter(typeof(PosTypeConverter))]
    public struct Pos
    {
        public float X;
        public float Y;
        public float Z;

        public Pos(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"<{X}, {Y}, {Z}>";
        }

        public static bool operator ==(Pos a, Pos b)
        {
            return (Math.Abs(a.X - b.X) < .005f) && (Math.Abs(a.Y - b.Y) < .005f) && (Math.Abs(a.Z - b.Z) < .005f);
        }

        public static bool operator !=(Pos a, Pos b)
        {
            return !(a == b);
        }

    }

    public struct NavigationPointLink
    {
        public Pos linkTo;
        public float costMultiplier;
    }

    public struct NavigationPoint
    {
        public Pos position;
        public List<NavigationPointLink> links;

        public void AddLink(Pos linkTo, float costMultiplier)
        {
            if(links.FindIndex(v => v.linkTo == linkTo) > -1)
                return;
            links.Add(new NavigationPointLink{
                linkTo = linkTo,
                costMultiplier = costMultiplier
            });
        }

        public void BreakLink(Pos linkToBreak)
        {
            var i = links.FindIndex(v => v.linkTo == linkToBreak);
            if(i > -1)
                return;
            links.RemoveAt(i);
        }
    }

    public class NavigationMap
    {
        public Dictionary<Pos, NavigationPoint> points = new Dictionary<Pos, NavigationPoint>();

        public void Reset()
        {
            points = new Dictionary<Pos, NavigationPoint>();
        }

        /*
         * @returns: true if the point at position exists
         */
        public bool HasPoint(Pos position)
        {
            return points.ContainsKey(position);
        }

        /*
         * @returns: true if the point has been added, false if it already exists.
         */
        public bool AddPoint(Pos position)
        {
            if(HasPoint(position))
                return false;
            points[position] = new NavigationPoint{position = position, links = new List<NavigationPointLink>()};
            return true;
        }

        /*
         * Ensures the passed points have bidirectional links.
         * @return: true if the link was successful, false if either of the points don't exist
         */
        public bool AddPointLink(Pos pointA, Pos pointB, float costMultiplier = 1f)
        {
            if(!HasPoint(pointA) || !HasPoint(pointB))
                return false;
            points[pointA].AddLink(pointB, costMultiplier);
            points[pointB].AddLink(pointA, costMultiplier);
            return true;
        }

        /*
         * Breaks any link between two passed points
         */
        public void BreakPointLink(Pos pointA, Pos pointB)
        {
            if(HasPoint(pointA))
                points[pointA].BreakLink(pointB);
            if(HasPoint(pointB))
                points[pointB].BreakLink(pointA);
        }
    }

    public class TilemapNavigation
    {

    }
}