﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using RatchetEdit.Headers;
using RatchetEdit.LevelObjects;
using static RatchetEdit.DataFunctions;

namespace RatchetEdit.Serializers
{
    class GameplaySerializer
    {
        public const int MOBYLENGTH = 0x78;


        public void Save(Level level, String fileName)
        {
            FileStream fs = File.Open(fileName, FileMode.Create);

            //Seek past the header
            fs.Seek(0xA0, SeekOrigin.Begin);

            GameplayHeader gameplayHeader = new GameplayHeader
            {
                type88Pointer =     SeekWrite(fs, GetType88Bytes(level.type88s)),
                levelVarPointer =   SeekWrite(fs, level.levelVariables.serialize()),
                englishPointer =    SeekWrite(fs, level.english),
                lang2Pointer =      SeekWrite(fs, level.lang2),
                frenchPointer =     SeekWrite(fs, level.french),
                germanPointer =     SeekWrite(fs, level.german),
                spanishPointer =    SeekWrite(fs, level.spanish),
                italianPointer =    SeekWrite(fs, level.italian),
                lang7Pointer =      SeekWrite(fs, level.lang7),
                lang8Pointer =      SeekWrite(fs, level.lang8),
                type04Pointer =     SeekWrite(fs, GetType04Bytes(level.type04s)),
                type80Pointer =     SeekWrite(fs, GetType80Bytes(level.type80s)),
                cameraPointer =     SeekWrite(fs, GetGameCameraBytes(level.gameCameras)),
                type0CPointer =     SeekWrite(fs, GetType0CBytes(level.type0Cs)),
                mobyIdPointer =     SeekWrite(fs, GetIdBytes(level.mobyIds)),
                mobyPointer =       SeekWrite(fs, GetMobyBytes(level.mobs)),
                pvarSizePointer =   SeekWrite(fs, GetPvarSizeBytes(level.pVars)),
                pvarPointer =       SeekWrite(fs, GetPvarBytes(level.pVars)),
                type50Pointer =     SeekWrite(fs, GetKeyValueBytes(level.type50s)),
                type5CPointer =     SeekWrite(fs, GetKeyValueBytes(level.type5Cs)),
                unkPointer6 =       SeekWrite(fs, level.unk6),
                unkPointer7 =       SeekWrite(fs, level.unk7),
                tieIdPointer =      SeekWrite(fs, GetIdBytes(level.tieIds)),
                tiePointer =        SeekWrite(fs, level.tieData),
                shrubIdPointer =    SeekWrite(fs, GetIdBytes(level.shrubIds)),
                shrubPointer =      SeekWrite(fs, level.shrubData),
                splinePointer =     SeekWrite(fs, GetSplineBytes(level.splines)),
                spawnPointPointer = SeekWrite(fs, GetSpawnPointBytes(level.cuboids)),
                type64Pointer =     SeekWrite(fs, GetType64Bytes(level.type64s)),
                type68Pointer =     SeekWrite(fs, GetType68Bytes(level.type68s)),
                unkPointer12 =      SeekWrite(fs, new byte[0x10]),
                unkPointer17 =      SeekWrite(fs, level.unk17),
                type7CPointer =     SeekWrite(fs, GetType7CBytes(level.type7Cs)),
                unkPointer14 =      SeekWrite(fs, level.unk14),
                unkPointer13 =      SeekWrite(fs, level.unk13),
                occlusionPointer =  SeekWrite(fs, GetOcclusionBytes(level.occlusionData))
            };

            //Seek to the beginning of the file to append the updated header
            byte[] head = gameplayHeader.Serialize();
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(head, 0, head.Length);

            fs.Close();
        }

        private int SeekWrite(FileStream fs, byte[] bytes)
        {
            if (bytes != null)
            {
                SeekPast(fs);
                int pos = (int)fs.Position;
                fs.Write(bytes, 0, bytes.Length);
                return pos;
            }
            else return 0;
        }

        private void SeekPast(FileStream fs)
        {
            while (fs.Position % 0x10 != 0)
            {
                fs.Seek(4, SeekOrigin.Current);
            }
        }

        public byte[] GetMobyBytes(List<Moby> mobs)
        {
            if (mobs == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x10 + mobs.Count * MOBYLENGTH];

            //Header
            WriteUint(ref bytes, 0, (uint)mobs.Count);
            WriteUint(ref bytes, 4, 0x100);

            for(int i = 0; i < mobs.Count; i++)
            {
                mobs[i].ToByteArray().CopyTo(bytes, 0x10 + i * MOBYLENGTH);
            }
            return bytes;
        }


        public byte[] GetSpawnPointBytes(List<Cuboid> spawnPoints)
        {
            if (spawnPoints == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x10 + spawnPoints.Count * 0x80];

            //Header
            WriteUint(ref bytes, 0, (uint)spawnPoints.Count);

            for (int i = 0; i < spawnPoints.Count; i++)
            {
                spawnPoints[i].ToByteArray().CopyTo(bytes, 0x10 + i * 0x80);
            }
            return bytes;
        }

        public byte[] GetGameCameraBytes(List<GameCamera> gameCameras)
        {
            if (gameCameras == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x10 + gameCameras.Count * GameCamera.ELEMENTSIZE];

            //Header
            WriteUint(ref bytes, 0, (uint)gameCameras.Count);

            for (int i = 0; i < gameCameras.Count; i++)
            {
                gameCameras[i].ToByteArray().CopyTo(bytes, 0x10 + i * GameCamera.ELEMENTSIZE);
            }

            return bytes;
        }

        public byte[] GetType04Bytes(List<Type04> type04s)
        {
            if (type04s == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x10 + type04s.Count * Type04.ELEMENTSIZE];

            //Header
            WriteInt(ref bytes, 0, type04s.Count);

            for (int i = 0; i < type04s.Count; i++)
            {
                type04s[i].ToByteArray().CopyTo(bytes, 0x10 + i * Type04.ELEMENTSIZE);
            }

            return bytes;
        }

        public byte[] GetType0CBytes(List<Type0C> type0Cs)
        {
            if (type0Cs == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x10 + type0Cs.Count * Type0C.ELEMENTSIZE];

            //Header
            WriteInt(ref bytes, 0, type0Cs.Count);

            for (int i = 0; i < type0Cs.Count; i++)
            {
                type0Cs[i].ToByteArray().CopyTo(bytes, 0x10 + i * Type0C.ELEMENTSIZE);
            }

            return bytes;
        }

        public byte[] GetType64Bytes(List<Type64> type64s)
        {
            if (type64s == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x10 + type64s.Count * Type64.ELEMENTSIZE];

            //Header
            WriteInt(ref bytes, 0, type64s.Count);

            for (int i = 0; i < type64s.Count; i++)
            {
                type64s[i].ToByteArray().CopyTo(bytes, 0x10 + i * Type64.ELEMENTSIZE);
            }

            return bytes;
        }

        public byte[] GetType68Bytes(List<Type68> type68s)
        {
            if (type68s == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x10 + type68s.Count * Type68.ELEMENTSIZE];

            //Header
            WriteInt(ref bytes, 0, type68s.Count);

            for (int i = 0; i < type68s.Count; i++)
            {
                type68s[i].ToByteArray().CopyTo(bytes, 0x10 + i * Type68.ELEMENTSIZE);
            }

            return bytes;
        }

        public byte[] GetType7CBytes(List<Type7C> type7Cs)
        {
            if (type7Cs == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x10 + type7Cs.Count * Type7C.ELEMENTSIZE];

            //Header
            WriteInt(ref bytes, 0, type7Cs.Count);

            for (int i = 0; i < type7Cs.Count; i++)
            {
                type7Cs[i].Serialize().CopyTo(bytes, 0x10 + i * Type7C.ELEMENTSIZE);
            }

            return bytes;
        }

        public byte[] GetType88Bytes(List<Type88> type88s)
        {
            if (type88s == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x10 + type88s.Count * Type88.ELEMENTSIZE];

            //Header
            WriteInt(ref bytes, 0, type88s.Count);

            for (int i = 0; i < type88s.Count; i++)
            {
                type88s[i].ToByteArray().CopyTo(bytes, 0x10 + i * Type88.ELEMENTSIZE);
            }

            return bytes;
        }


        public byte[] GetType80Bytes(List<Type80> type80s)
        {
            if (type80s == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x10 + type80s.Count * (Type80.HEADSIZE + Type80.DATASIZE)];

            //Header
            WriteInt(ref bytes, 0, type80s.Count);

            for (int i = 0; i < type80s.Count; i++)
            {
                byte[] headBytes = type80s[i].SerializeHead();
                byte[] dataBytes = type80s[i].SerializeData();

                headBytes.CopyTo(bytes, 0x10 + i * Type80.HEADSIZE);
                dataBytes.CopyTo(bytes, 0x10 + Type80.HEADSIZE * type80s.Count + i * Type80.DATASIZE);
            }
            return bytes;
        }

        public byte[] GetKeyValueBytes(List<KeyValuePair<int, int>> type50s)
        {
            if (type50s == null) { return new byte[0x10]; }

            byte[] bytes = new byte[type50s.Count * 8 + 0x08];

            int idx = 0;
            foreach(KeyValuePair<int, int> pair in type50s)
            {
                WriteInt(ref bytes, idx * 8 + 0, pair.Key);
                WriteInt(ref bytes, idx * 8 + 4, pair.Value);
                idx++;
            }

            WriteInt(ref bytes, bytes.Length - 8, -1);
            WriteInt(ref bytes, bytes.Length - 4, -1);
            return bytes;
        }

        public byte[] GetIdBytes(List<int> ids)
        {
            if (ids == null) { return new byte[0x10]; }

            byte[] bytes = new byte[0x04 + ids.Count * 4];
            BitConverter.GetBytes(ids.Count).CopyTo(bytes, 0);
            for (int i = 0; i < ids.Count; i++)
            {
                BitConverter.GetBytes(ids[i]).CopyTo(bytes, 0x04 + i * 0x04);
            }
            return bytes;
        }


        public byte[] GetSplineBytes(List<Spline> splines)
        {
            if (splines == null) { return new byte[0x10]; }

            List<byte> splineData = new List<byte>();
            List<int> offsets = new List<int>();

            int offset = 0;
            foreach (Spline spline in splines)
            {
                byte[] splineBytes = spline.ToByteArray();
                splineData.AddRange(splineBytes);
                offsets.Add(offset);
                offset += splineBytes.Length;
            }

            byte[] offsetBlock = new byte[GetLength(offsets.Count * 4)];
            for (int i = 0; i < offsets.Count; i++)
            {
                WriteUint(ref offsetBlock, i * 4, (uint)offsets[i]);
            }

            var bytes = new byte[0x10 + offsetBlock.Length + splineData.Count];
            WriteUint(ref bytes, 0,     (uint)splines.Count);
            WriteUint(ref bytes, 0x04,  (uint)(0x10 + offsetBlock.Length));
            WriteUint(ref bytes, 0x08,  (uint)(splineData.Count));
            offsetBlock.CopyTo(bytes, 0x10);
            splineData.CopyTo(bytes, 0x10 + offsetBlock.Length);

            return bytes;
        }

        public byte[] GetPvarSizeBytes(List<byte[]> pVars)
        {
            if (pVars == null) { return new byte[0x10]; }

            byte[] bytes = new byte[pVars.Count * 8];
            uint offset = 0;
            for (int i = 0; i < pVars.Count; i++)
            {
                WriteUint(ref bytes, (i * 8) + 0x00, offset);
                WriteUint(ref bytes, (i * 8) + 0x04, (uint)pVars[i].Length);
                offset += (uint)pVars[i].Length;
            }
            return bytes;
        }

        public byte[] GetPvarBytes(List<byte[]> pVars)
        {
            if (pVars == null) { return new byte[0x10]; }

            var bytes = new byte[pVars.Sum(arr => arr.Length)];
            int index = 0;
            foreach (var pVar in pVars)
            {
                pVar.CopyTo(bytes, index);
                index += pVar.Length;
            }

            return bytes;
        }

        public byte[] GetOcclusionBytes(OcclusionData occlusionData)
        {
            if (occlusionData == null) { return new byte[0x10]; }

            return occlusionData.ToByteArray();
        }

    }
}
