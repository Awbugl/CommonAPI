﻿using System;
using System.Collections.Generic;
using System.IO;

namespace CommonAPI
{
    public class FactorySystemStorage : ISerializeState
    {
        public List<IFactorySystem> planetSystems = new List<IFactorySystem>();
        public int typeIndex;
        
        public void Init(GameData data, int index)
        {
            planetSystems.Capacity = data.factoryCount;
            typeIndex = index;
            
            for (int i = 0; i < data.factoryCount; i++)
            {
                planetSystems.Add(CustomFactory.systemRegistry.GetNew(index));
                planetSystems[i].Init(data.factories[i]);
            }
        }
        //TODO make sure when new planets are added that system adds then here too

        public IFactorySystem GetSystem(PlanetFactory factory)
        {
            return planetSystems[factory.index];
        }

        public void PreUpdate(PlanetFactory factory)
        {
            if (GetSystem(factory) is IPreUpdate pre)
            {
                pre.PreUpdate();
            }
        }
        
        public void Update(PlanetFactory factory)
        {
            if (GetSystem(factory) is IUpdate update)
            {
                update.Update();
            }
        }
        
        public void PostUpdate(PlanetFactory factory)
        {
            if (GetSystem(factory) is IPostUpdate update)
            {
                update.PostUpdate();
            }
        }
        
        public void PowerUpdate(PlanetFactory factory)
        {
            if (GetSystem(factory) is IPowerUpdate update)
            {
                update.PowerUpdate();
            }
        }

        public bool PreUpdateSupportsMultithread()
        {
            return planetSystems[0] is IPreUpdateMultithread;
        }
        
        public bool UpdateSupportsMultithread()
        {
            return planetSystems[0] is IUpdateMultithread;
        }
        
        public bool PostUpdateSupportsMultithread()
        {
            return planetSystems[0] is IPostUpdateMultithread;
        }
        
        public bool PowerUpdateSupportsMultithread()
        {
            return planetSystems[0] is IPowerUpdateMultithread;
        }
        
        public void PreUpdateMultithread(PlanetFactory factory, int usedThreadCount, int currentThreadIdx, int minimumCount)
        {
            if (GetSystem(factory) is IPreUpdateMultithread pre)
            {
                pre.PreUpdateMultithread(usedThreadCount, currentThreadIdx, minimumCount);
            }
        }

        public void UpdateMultithread(PlanetFactory factory, int usedThreadCount, int currentThreadIdx, int minimumCount)
        {
            if (GetSystem(factory) is IUpdateMultithread update)
            {
                update.UpdateMultithread(usedThreadCount, currentThreadIdx, minimumCount);
            }
        }
        
        public void PostUpdateMultithread(PlanetFactory factory, int usedThreadCount, int currentThreadIdx, int minimumCount)
        {
            if (GetSystem(factory) is IPostUpdateMultithread update)
            {
                update.PostUpdateMultithread(usedThreadCount, currentThreadIdx, minimumCount);
            }
        }
        
        public void PowerUpdateMultithread(PlanetFactory factory, int usedThreadCount, int currentThreadIdx, int minimumCount)
        {
            if (GetSystem(factory) is IPowerUpdateMultithread update)
            {
                update.PowerUpdateMultithread(usedThreadCount, currentThreadIdx, minimumCount);
            }
        }
        
        public void Free()
        {
            foreach (IFactorySystem system in planetSystems)
            {
                system.Free();
            }
            planetSystems.Clear();
            planetSystems = null;
        }

        public void Export(BinaryWriter w)
        {
            GameData data = GameMain.data;

            w.Write(0);
            
            CommonAPIPlugin.logger.LogInfo("Start System storage Export");

            for (int i = 0; i < data.factoryCount; i++)
            {
                planetSystems[i].Export(w);
            }
        }

        public void Import(BinaryReader r)
        {
            GameData data = GameMain.data;
            
            CommonAPIPlugin.logger.LogInfo("Start System storage Import");

            int ver = r.ReadInt32();
            
            planetSystems.Clear();
            planetSystems.Capacity = data.factoryCount;

            for (int i = 0; i < data.factoryCount; i++)
            {
                planetSystems.Add(CustomFactory.systemRegistry.GetNew(typeIndex));
                planetSystems[i].Init(data.factories[i]);
                planetSystems[i].Import(r);
            }
        }
    }
}