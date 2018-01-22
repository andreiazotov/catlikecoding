using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public int AverageFPS { get; private set; }
    public int HighestFPS { get; private set; }
    public int LowestFPS { get; private set; }

    public int frameRange = 60;

    private int[] _fpsBuffer;
    private int _fpsBufferIndex;

    private void Update()
    {
        if (this._fpsBuffer == null || this._fpsBuffer.Length != this.frameRange)
        {
            this.InitializeBuffer();
        }
        this.UpdateBuffer();
        this.CalculateFPS();
    }

    private void UpdateBuffer()
    {
        this._fpsBuffer[this._fpsBufferIndex++] = (int)(1.0f / Time.unscaledDeltaTime);
        if (this._fpsBufferIndex >= this.frameRange)
        {
            this._fpsBufferIndex = 0;
        }
    }

    private void CalculateFPS()
    {
        int sum = 0;
        int highest = 0;
        int lowest = int.MaxValue;
        foreach (var el in this._fpsBuffer)
        {
            sum += el;
            if (el > highest)
            {
                highest = el;
            }
            if (el < lowest)
            {
                lowest = el;
            }
        }
        this.AverageFPS = sum / frameRange;
        this.HighestFPS = highest;
        this.LowestFPS = lowest;
    }

    private void InitializeBuffer()
    {
        if (this.frameRange <= 0)
        {
            this.frameRange = 1;
        }
        this._fpsBuffer = new int[this.frameRange];
        this._fpsBufferIndex = 0;
    }
}
