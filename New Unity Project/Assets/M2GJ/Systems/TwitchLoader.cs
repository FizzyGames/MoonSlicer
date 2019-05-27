using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using MoonSlicer.Components;
using System.Linq;

public class TwitchLoader : MonoBehaviour
{
    private string _clientID = "rhi6ewiznm4dlgi87nefo503wof409";
    private string _channelName = "moonmoon_ow";
    string[] _chatters;
    public string[] Chatters => _chatters;
    // Start is called before the first frame update

    public GameObject MeleeEnemy;
    public GameObject RangedEnemy;

    public int Size = 500;

    void Start()
    {
        //_api = new Api();
        //_api.Settings.ClientId = _clientID;
        ////Chat = new Chat(_api);
        //_client = new Client();
        //_client.Initialize(new TwitchLib.Client.Models.ConnectionCredentials("justinfan1234", ""), _channelName);
        //_client.OnMessageReceived += _client_OnMessageReceived;
        //_client.OnConnected += _client_OnConnected;
        //_client.OnExistingUsersDetected += _client_OnExistingUsersDetected;
        //_client.Connect();
        //Debug.Log(_client.IsConnected);
        //_api.Streams.helix.
        //_api.Invoke(_api.Chat.v5.GetChatRoomsByChannelAsync("moonmoon_ow"));
        WWWForm form = new WWWForm();
        StartCoroutine(GetAllUsers());
    }

    private IEnumerator GetAllUsers()
    {

        using (var w = UnityWebRequest.Get($"http://tmi.twitch.tv/group/user/{_channelName}/chatters"))
        {
            yield return w.SendWebRequest();
            Debug.Log("Connected!");
            var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatterList>(w.downloadHandler.text);
            var melee = GameObjectConversionUtility.ConvertGameObjectHierarchy(MeleeEnemy, World.Active);
            var ranged = GameObjectConversionUtility.ConvertGameObjectHierarchy(RangedEnemy, World.Active);
            var entityManager = World.Active.EntityManager;
            var meleeState = entityManager.GetComponentData<EnemyState>(melee);
            var rangedState = entityManager.GetComponentData<EnemyState>(ranged);
            _chatters = deserialized.chatters.viewers.ToArray();
            for (int index = 0; index < _chatters.Length; ++index)
            {
                bool isMelee = UnityEngine.Random.value > 0.5f;
                var instance = entityManager.Instantiate(isMelee ? melee : ranged);

                Vector2 position2D = UnityEngine.Random.insideUnitCircle * Size;
                float3 position = new float3(position2D.x, 0, position2D.y);
                entityManager.SetComponentData(instance, new Translation { Value = position });
                meleeState.NameID = rangedState.NameID = index;
                entityManager.SetComponentData(instance, isMelee ? meleeState : rangedState);
            }
        }
    }

    [Serializable]
    public class ChatterList
    {
        public int chatter_count;
        public ChatterSubList chatters = new ChatterSubList();
    }

    [Serializable]
    public class ChatterSubList
    {
        public IList<string> broadcaster;
        public IList<string> vips;
        public IList<string> moderators;
        public IList<string> staff;
        public IList<string> admins;
        public IList<string> global_mods;
        public IList<string> viewers;
    }
}
