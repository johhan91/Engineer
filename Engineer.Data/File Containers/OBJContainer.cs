﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Engineer.IO;

namespace Engineer.Data
{
    public class OBJContainer : MeshContainer
    {
        private char[] _WhiteSpaces = { ' ', '\t' };
        private string[] CleanStringArray(string[] Array)
        {
            List<string> NewArray = new List<string>();
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i] != "") NewArray.Add(Array[i]);
            }
            return NewArray.ToArray();
        }
        public override void Load(string FilePath, BackgroundWorker Worker)
        {
            FileStream Stream = new FileStream(FilePath, FileMode.Open);
            StreamReader Reader = new StreamReader(Stream);
            Geometry NewGeometry = null;
            while (!Reader.EndOfStream)
            {
                string Line = Reader.ReadLine();
                string[] Parts = CleanStringArray(Line.Split(_WhiteSpaces));
                if (Line.StartsWith("#")) continue;
                if (Line.StartsWith("g") || Line.StartsWith("o"))
                {
                    if (NewGeometry != null) this.Geometries.Add(NewGeometry);
                    NewGeometry = new Geometry(Parts[1]);
                    continue;
                }
                if (Line.StartsWith("vn"))
                {
                    NewGeometry.Normals.Add(new Mathematics.Vertex(Converter.ToSingle(Parts[1]), Converter.ToSingle(Parts[2]), Converter.ToSingle(Parts[3])));
                    continue;
                }
                if (Line.StartsWith("vt"))
                {
                    NewGeometry.TexCoords.Add(new Mathematics.Vertex(Converter.ToSingle(Parts[1]), Converter.ToSingle(Parts[2]), 0));
                    continue;
                }
                if (Line.StartsWith("v"))
                {
                    NewGeometry.Vertices.Add(new Mathematics.Vertex(Converter.ToSingle(Parts[1]), Converter.ToSingle(Parts[2]), Converter.ToSingle(Parts[3])));
                    continue;
                }
                if (Line.StartsWith("f"))
                {
                    Mathematics.Face NewFace = new Mathematics.Face();
                    for(int i = 1; i < Parts.Length; i++)
                    {
                        string[] SubParts = Parts[i].Split('/');
                        NewFace.Vertices.Add(Math.Abs(Convert.ToInt32(SubParts[0]))-1);
                        if (SubParts.Length > 1) NewFace.TexCoords.Add(Math.Abs(Convert.ToInt32(SubParts[1]))-1);
                        if (SubParts.Length > 2) NewFace.Normals.Add(Math.Abs(Convert.ToInt32(SubParts[2])) - 1);
                    }
                    NewGeometry.Faces.Add(NewFace);
                    continue;
                }
            }
            this.Geometries.Add(NewGeometry);
            Reader.Close();
        }
    }
}
