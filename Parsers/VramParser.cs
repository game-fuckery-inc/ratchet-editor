﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RatchetEdit.DataFunctions;

namespace RatchetEdit {
    class VramParser {
        FileStream fileStream;

        public VramParser(string filepath) {
            try {
                fileStream = File.OpenRead(filepath);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                MessageBox.Show("vram.ps3 missing!", "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();
                return;
            }
        }

        public void GetTextures(List<Texture> textures) {
            for (int i = 0; i < textures.Count; i++) {
                int length = 0;
                if (i < textures.Count - 1) {
                    length = (int)(textures[i + 1].vramPointer - textures[i].vramPointer);
                }
                else {
                    length = (int)(fileStream.Length - textures[i].vramPointer);
                }
                textures[i].data = ReadBlock(fileStream, textures[i].vramPointer, length);
            }
        }

        public void Close() {
            fileStream.Close();
        }
    }
}
