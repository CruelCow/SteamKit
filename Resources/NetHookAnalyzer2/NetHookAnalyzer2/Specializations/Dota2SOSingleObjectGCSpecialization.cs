﻿using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;
using SteamKit2.GC.Dota.Internal;
using SteamKit2.GC.Internal;

namespace NetHookAnalyzer2.Specializations
{
    class Dota2SOSingleObjectGCSpecialization : IGameCoordinatorSpecialization
    {
        const uint Dota2AppID = 570;

        public IEnumerable<KeyValuePair<string, object>> GetExtraObjects(object body, uint appID)
        {
            if (appID != Dota2AppID)
            {
                yield break;
            }

            var updateSingle = body as CMsgSOSingleObject;
            if (updateSingle == null)
            {
                yield break;
            }

            var extraNode = ReadExtraObject(updateSingle);
            if (extraNode != null)
            {
                yield return new KeyValuePair<string, object>("SO", extraNode);
            }
        }

        object ReadExtraObject(CMsgSOSingleObject sharedObject)
        {
            try
            {
                using (var ms = new MemoryStream(sharedObject.object_data))
                {
                    Type t;
                    if (Dota2SOHelper.SOTypes.TryGetValue(sharedObject.type_id, out t))
                    {
                            return RuntimeTypeModel.Default.Deserialize(ms, null, t);
                    }
                }
            }
            catch (ProtoException ex)
            {
                return "Error parsing SO data: " + ex.Message;
            }

            return null;
        }
    }
}