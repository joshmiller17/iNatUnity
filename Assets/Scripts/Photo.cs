using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JoshAaronMiller.INaturalist
{
    public class Photo
    {
        public string filePath;
        public string fileName;
        public string fileType; // e.g., "image/png"

        public byte[] ToBytes()
        {
            return File.ReadAllBytes(filePath);
        }
    }
}
