using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace MoonSlicer.Systems
{
    public class KillFeedController : MonoBehaviour
    {
        int slayed = 0;
        TwitchLoader TwitchLoader { get; set; }
        public object StringBuilder { get; private set; }

        TextMeshProUGUI text;

        List<int> _lastTenIDs = new List<int>();

        private void Start()
        {
            //this is terrible, but again time constraints
            TwitchLoader = FindObjectOfType<TwitchLoader>();
            text = GetComponent<TextMeshProUGUI>();
            slayed = 0;
        }

        public void RecordSlay(int id)
        {
            _lastTenIDs.Add(id);
            slayed++;
        }

        private void Update()
        {
            StringBuilder newText = new StringBuilder();
            if (_lastTenIDs.Count > 10)
                _lastTenIDs.RemoveRange(0, _lastTenIDs.Count - 10);
            int max = Mathf.Min(10, _lastTenIDs.Count);
            newText.Append($"Twitch chat slayed: <color=blue>{slayed}</color> Remaining: <color=blue>{(TwitchLoader.Chatters?.Length ?? 0) - slayed}</color>\r\n");
            for (int i = 0; i < max; i++)
            {
                newText.Append($"Slayed \"<color=green>{TwitchLoader.Chatters[_lastTenIDs[i]]}</color>\"!\r\n");
            }
            text.text = newText.ToString();
        }
    }
}