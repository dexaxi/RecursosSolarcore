using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightPattern
{
    private Texture2D _patternTexture;

    private int[,] _textureMatrix;

    public HighlightPattern(Texture2D patternTexture)
    {
        SetPatternTextureAndForcePrecalc(patternTexture);
    }

    private void SetPatternTextureAndForcePrecalc(Texture2D patternTexture)
    {
        _patternTexture = patternTexture;
        PrecalcTexture();
    }

    private void PrecalcTexture() 
    {
        _textureMatrix = new int[_patternTexture.width, _patternTexture.height];
        Color[] pixels = _patternTexture.GetPixels();
        for (int i = 0; i < _patternTexture.width; i++) 
        {
            for (int j = 0; j < _patternTexture.height; j++) 
            {
                Color currentPixel = pixels[i + (j * _patternTexture.width)];
                _textureMatrix[i, j] = currentPixel == Color.white ? 1 : 0;
            }
        }
    }

    public int[,] GetPattern() 
    {
        return _textureMatrix;
    }
}
