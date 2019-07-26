using UnityEngine;
using System.Collections.Generic;

public class SoundController : MonoBehaviour
{
    private Dictionary<string, AudioClip> sounds;

    private float soundCooldown = 0;

    // Use this for initialization
    void Start()
    {
        sounds = new Dictionary<string, AudioClip>();

        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds");
        foreach(AudioClip c in clips)
        {
            sounds.Add(c.name, c);
        }

        World.current.cbInstalledObjectCreated += OnInstalledObjectCreated;
        World.current.cbTileChanged += OnTileChanged;
    }

    private void Update()
    {
        soundCooldown -= Time.deltaTime;
    }

    void OnTileChanged(Tile tile)
    {       
        if (soundCooldown > 0) return;

        if(tile == null) return;        

        if(tile.Type == TileType.Floor && sounds.ContainsKey("Floor_OnCreated"))
        {
            soundCooldown = 0.1f;
            AudioSource.PlayClipAtPoint(sounds["Floor_OnCreated"], new Vector3(tile.x, tile.y, 0));
        }
    }

    void OnInstalledObjectCreated(InstalledObject obj)
    {
        if (soundCooldown > 0) return;

        if(obj == null) return;        

        AudioSource.PlayClipAtPoint(sounds["Wall_OnCreated"], new Vector3(obj.tile.x, obj.tile.y, 0));
    }
}
