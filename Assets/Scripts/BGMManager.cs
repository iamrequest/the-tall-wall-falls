﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour {
    private AudioSource audioSource;
    public List<AudioClip> songs;

    [Range(0f, 1f)]
    public List<float> songVolumes;
    public int initialSongIndex;
    public int currentSongIndex;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        PlaySong(initialSongIndex);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            NextSong();
        } else if (Input.GetKeyDown(KeyCode.M)) {
            PreviousSong();
        }
    }

    public void NextSong() {
        PlaySong((currentSongIndex + 1) % songs.Count);
    }
    public void PreviousSong() {
        if (currentSongIndex - 1 < 0) {
            PlaySong(songs.Count - 1);
        } else {
            PlaySong(currentSongIndex - 1);
        }
    }

    public void PlaySong(int index) {
        if (songs.Count < 0) {
            Debug.LogError("Attempted to play BGM, but no songs are available");
            return;
        }

        currentSongIndex = index;
        audioSource.clip = songs[currentSongIndex];

        // Set song volume
        if (songVolumes.Count - 1 < currentSongIndex) {
            Debug.LogError("No available volume for this song, defaulting to 1");
            audioSource.volume = 1f;
        } else {
            audioSource.volume = songVolumes[currentSongIndex];
        }

        audioSource.Play();
    }
}