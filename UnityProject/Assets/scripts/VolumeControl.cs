using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string volumeParameters = "MasterVolume";
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider volumeSlider;
    float multiplier = 30f;

    private void Awake()
    {
        volumeSlider.onValueChanged.AddListener(HandleSliderValueChange);
    }
    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(volumeParameters, volumeSlider.value);
        HandleSliderValueChange(volumeSlider.value);
    }
    private void HandleSliderValueChange(float value)
    {
        audioMixer.SetFloat(volumeParameters, Mathf.Log10(value) * multiplier);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameters, volumeSlider.value);
    }


}
