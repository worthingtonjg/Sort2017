using CrazyMinnow.SALSA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CommunicationBehavior : MonoBehaviour {
	
	private AudioSource audioSource;
    public Salsa3D salsa;

	public void Start()
	{
        GameObject actor = GameObject.Find("female_missionary");

        audioSource = actor.GetComponent<AudioSource>();
        salsa = actor.GetComponent<Salsa3D>();
    }

	public void PlayAudio(byte[] bytes)
	{
		int sampleCount = 0;
		int frequency = 0;
		var unitAudio = ToUnityAudio (bytes, out sampleCount, out frequency);

		var clip = ToClip ("Speech", unitAudio, sampleCount, frequency);
		audioSource.clip = clip;
		salsa.Play ();
	}

	private float[] ToUnityAudio(byte[] wavAudio, out int sampleCount, out int frequency)
	{
		// Determine if mono or stereo
		int channelCount = wavAudio[22];  // Speech audio data is always mono but read actual header value for processing

		// Get the frequency
		frequency = BytesToInt(wavAudio, 24);

		// Get past all the other sub chunks to get to the data subchunk:
		int pos = 12; // First subchunk ID from 12 to 16

		// Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
		while (!(wavAudio[pos] == 100 && wavAudio[pos + 1] == 97 && wavAudio[pos + 2] == 116 && wavAudio[pos + 3] == 97))
		{
			pos += 4;
			int chunkSize = wavAudio[pos] + wavAudio[pos + 1] * 256 + wavAudio[pos + 2] * 65536 + wavAudio[pos + 3] * 16777216;
			pos += 4 + chunkSize;
		}

		pos += 8;

		// Pos is now positioned to start of actual sound data.
		sampleCount = (wavAudio.Length - pos) / 2;  // 2 bytes per sample (16 bit sound mono)

		if (channelCount == 2) { sampleCount /= 2; }  // 4 bytes per sample (16 bit stereo)

		// Allocate memory (supporting left channel only)
		var unityData = new float[sampleCount];

		// Write to double array/s:
		int i = 0;

		while (pos < wavAudio.Length)
		{
			unityData[i] = BytesToFloat(wavAudio[pos], wavAudio[pos + 1]);
			pos += 2;

			if (channelCount == 2)
			{
				pos += 2;
			}

			i++;
		}
			
		return unityData;
	}

	private AudioClip ToClip(string name, float[] audioData, int sampleCount, int frequency)
	{
		var clip = AudioClip.Create (name, sampleCount, 1, frequency, false);
		clip.SetData (audioData, 0);
		return clip;
	}

	private float BytesToFloat(byte firstByte, byte secondByte)
	{
		// Convert two bytes to one short (little endian)
		short s = (short)((secondByte << 8) | firstByte);

		// Convert to range from -1 to (just below) 1
		return s / 32768.0F;
	}

	private int BytesToInt(byte[] bytes, int offset = 0)
	{
		int value = 0;
		for (int i = 0; i < 4; i++)
		{
			value |= ((int)bytes[offset + i]) << (i * 8);
		}
		return value;
	}

}
