﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Linq;

namespace EdgeLibrary
{
        /// <summary>
        /// Used when changing a Texture2D - passed in to identify which pixels should be changed
        /// </summary>
        public class TextureEditParams
        {
            //Selects pixels with an average of R+G+B between Min and Max darkness
            public byte MinDarkness;
            public byte MaxDarkness;

            //Set to true if Min/Max Color should be used
            public bool UseColor;
            //Selects pixels with a color between min and max colors
            public Color MaxColor { get { return _maxColor; } set { _maxColor = value; UseColor = true; } }
            private Color _maxColor;
            public Color MinColor { get { return _minColor; } set { _minColor = value; UseColor = true; } }
            private Color _minColor;

            //Set to true if Area should be used
            public bool UseArea;
            //Selects pixels from a certain area on the texture
            public Rectangle Area { get { return _area; } set { _area = value; UseArea = true; } }
            private Rectangle _area;

            public TextureEditParams()
            {
                MinDarkness = 0;
                MaxDarkness = 255;
                UseColor = false;
                MinColor = Color.White;
                MaxColor = MinColor;
                UseArea = false;
            }
        }

        /// <summary>
        /// Used when changing a Texture2D - passed in to identify how to change each pixel
        /// </summary>
        public class TextureChangeParams
        {
            //Set to true to use Min/Max Color
            public bool UseColor;
            //Selects a random color between Min and Max color
            public Color MaxColor { get { return _maxColor; } set { _maxColor = value; UseColor = true; } }
            private Color _maxColor;
            public Color MinColor { get { return _minColor; } set { _minColor = value; UseColor = true; } }
            private Color _minColor;

            //Adds this value to the color's R, G, and B
            public int Add;

            //Colorizes the texture by this color and value
            public Color ColorizeColor;
            public float ColorizeValue;

            public TextureChangeParams()
            {
                UseColor = false;
                MinColor = Color.White;
                MaxColor = MinColor;
                Add = 0;
                ColorizeColor = Color.White;
                ColorizeValue = 0;
            }
        }

        public static class TextureTools
        {
            #region GENERATING TOOLS
            //Makes a copy of a Texture2D
            public static Texture2D Copy(Texture2D texture)
            {
                Color[] colors = new Color[texture.Width * texture.Height];
                texture.GetData<Color>(colors);
                Texture2D returnTexture = new Texture2D(EdgeGame.GetCurrentGame().GraphicsDevice, texture.Width, texture.Height);
                returnTexture.SetData<Color>(colors);
                return returnTexture;
            }
            //Colorizes a Texture
            public static void Colorize(Texture2D texture, Color color, float factor)
            {
                Color[] colorData = new Color[texture.Width * texture.Height];
                texture.GetData<Color>(colorData);

                for (int i = 0; i < colorData.Length; i++)
                {
                    Color currentColor = colorData[i];
                    byte darkness = (byte)((currentColor.R + currentColor.G + currentColor.B) / 3);
                    colorData[i] = Color.Lerp(currentColor, color, ((float)darkness / 255) * factor * 2);
                }

                texture.SetData<Color>(colorData);
            }
            //Sets the inner rectangle portion of a Texture2D
            public static void SetInnerTexture(Texture2D texture, Texture2D innerTexture, Vector2 startPosition)
            {
                Color[] colorData = new Color[texture.Width * texture.Height];
                texture.GetData<Color>(colorData);
                Color[] innerColorData = new Color[innerTexture.Width * innerTexture.Height];
                innerTexture.GetData<Color>(innerColorData);

                for (int y = 0; y < innerTexture.Height; y++)
                {
                    for (int x = 0; x < innerTexture.Width; x++)
                    {
                        if (((x + (int)startPosition.X) + (y + (int)startPosition.Y) * texture.Width) < colorData.Length)
                        {
                            colorData[(x + (int)startPosition.X) + (y + (int)startPosition.Y) * texture.Width] = innerColorData[x + y * innerTexture.Width];
                        }
                    }
                }

                texture.SetData<Color>(colorData);
            }
            #endregion

            #region SPLITTING TOOLS
            //Gets an inner rectangle portion of a Texture2D
            public static Texture2D GetInnerTexture(Texture2D texture, Rectangle rectangle)
            {
                Texture2D returnTexture = EdgeGame.CreateNewTexture(rectangle.Width, rectangle.Height);
                Color[] colorData = new Color[texture.Width * texture.Height];
                texture.GetData<Color>(colorData);

                Color[] color = new Color[rectangle.Width * rectangle.Height];
                for (int y = 0; y < rectangle.Height; y++)
                {
                    for (int x = 0; x < rectangle.Width; x++)
                    {
                        color[x + y * rectangle.Width] = colorData[x + rectangle.X + (y + rectangle.Y) * texture.Width];
                    }
                }

                returnTexture.SetData<Color>(color);
                return returnTexture;
            }

            //Splits a spritesheet using an xml document and a texture
            public static Dictionary<string, Texture2D> SplitSpritesheet(Texture2D spriteSheetTexture, string XMLPath)
            {
                Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
                string completePath = string.Format("{0}\\{1}", EdgeGame.GetCurrentResources().ContentRootDirectory, XMLPath);
                XDocument textureResourceData = XDocument.Load(completePath);

                foreach (XElement element in textureResourceData.Root.Elements())
                {
                    Rectangle rectangle = new Rectangle(int.Parse(element.Attribute("x").Value), int.Parse(element.Attribute("y").Value), int.Parse(element.Attribute("width").Value), int.Parse(element.Attribute("height").Value));
                    textures.Add(element.Attribute("name").Value, GetInnerTexture(spriteSheetTexture, rectangle));
                }

                return textures;
            }
            public static Dictionary<string, Texture2D> SplitSpritesheet(string spriteSheetTexture, string XMLPath)
            {
                return SplitSpritesheet(EdgeGame.GetCurrentResources().TextureFromString(spriteSheetTexture), XMLPath);
            }
            #endregion

            #region EXTRA EDITING
            /// <summary>
            /// WORK IN PROGRESS - allows a texture to be modified
            /// </summary>
            public static void EditTexture(Texture2D texture, TextureEditParams editParams, TextureChangeParams changeParams)
            {
                editParams.Area = MathTools.ResolveNegativeRectangle(editParams.Area);

                Color[] colors = new Color[texture.Width * texture.Height];
                texture.GetData<Color>(colors);

                for (int i = 0; i < colors.Length; i++)
                {
                    //Checks if it's inside the rectangle
                    if ((editParams.UseArea == false || (i > editParams.Area.Left + editParams.Area.Top * texture.Width && i < editParams.Area.Right + editParams.Area.Bottom * texture.Width)))
                    {
                        //Checks if it matches the darkness
                        if ((colors[i].R + colors[i].G + colors[i].B) / 3 >= editParams.MinDarkness && (colors[i].R + colors[i].G + colors[i].B) / 3 <= editParams.MaxDarkness)
                        {
                            //Checks if it matches the min/max color
                            if ((colors[i].R >= editParams.MinColor.R && colors[i].R <= editParams.MaxColor.R) && (colors[i].G >= editParams.MinColor.G && colors[i].G <= editParams.MaxColor.G) && (colors[i].B >= editParams.MinColor.B && colors[i].B <= editParams.MaxColor.B))
                            {
                                if (changeParams.UseColor)
                                {
                                    colors[i] = Color.Lerp(changeParams.MinColor, changeParams.MaxColor, RandomTools.RandomFloat(0, 1));
                                }

                                colors[i] = MathTools.AddToColor(colors[i], changeParams.Add);

                                byte darkness = (byte)((colors[i].R + colors[i].G + colors[i].B) / 3);
                                colors[i] = Color.Lerp(colors[i], changeParams.ColorizeColor, ((float)darkness / 255) * changeParams.ColorizeValue * 2);
                            }
                        }
                    }
                }

                texture.SetData<Color>(colors);
            }
            #endregion
        }
}
