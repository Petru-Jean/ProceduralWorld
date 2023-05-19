using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UvMapper
{
    private static Dictionary<TextureConfiguration, Vector2[]> cache;

    private static Texture2D textureAtlas;
    private static int texLen = 4;
    private static int texCount = texLen * texLen; 

    static float texWidth = 16.0f;
    static float padding = (1.0f / texLen) * (2.0f / texWidth); 
    
    struct TextureConfiguration
    {
        public int textureId;
        public bool topPadding, bottomPadding, leftPadding, rightPadding;
    };

    static UvMapper()
    {
        textureAtlas = (Texture2D) Resources.Load("Textures/TextureAtlas");
        cache        = new Dictionary<TextureConfiguration, Vector2[]>();
        
    }

    public static Vector2[] GetUvs(int textureId, bool removeTopPadding, bool removeRightPadding, bool removeLeftPadding, bool removeBottomPadding)
    {
        if(textureId < 0 || textureId >= texCount)
            throw new UnityException("Attempt to get invalid texture [id: " + textureId + "]");

        TextureConfiguration config = new TextureConfiguration();
        config.textureId = textureId;

        config.bottomPadding = removeBottomPadding;
        config.topPadding    = removeTopPadding;
        config.leftPadding   = removeLeftPadding;
        config.rightPadding  = removeRightPadding;

        Vector2[] uvs;

        if(cache.TryGetValue(config, out uvs))
        {
            return uvs;
        }

        uvs = new Vector2[4];

        float texScale = 1.0f / texLen; 
 
        float xLeft = (float)(textureId % texLen) / (float)texLen; 
        float xRight   = xLeft + texScale; 

        float yDown = 1.0f - (float)((int)(textureId / texLen ) / (float)texLen) - texScale;
        float yUp   = yDown + texScale; 

        uvs[0] = new Vector2(xLeft + (removeLeftPadding ? padding : 0), yDown+ (removeBottomPadding ? padding : 0));
        uvs[1] = new Vector2(xRight - (removeRightPadding ? padding : 0), yDown + (removeBottomPadding ? padding : 0));
        uvs[2] = new Vector2(xLeft + (removeLeftPadding ? padding : 0), yUp - (removeTopPadding ? padding : 0));
        uvs[3] = new Vector2(xRight - (removeRightPadding ? padding : 0), yUp - (removeTopPadding ? padding : 0));

        cache[config] = uvs;

        return uvs;
    }

}
