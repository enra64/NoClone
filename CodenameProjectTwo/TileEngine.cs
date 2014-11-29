﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SFML;
using SFML.Window;
using SFML.Graphics;
using System.Xml.Linq;
using System.Diagnostics;

namespace CodenameProjectTwo
{
    class TileEngine
    {
        public RenderWindow win;
        public Vector2u tileAmount, tilesPerView = new Vector2u(100, 100);

        //tile specific variables
        private int[,] tileTypes;
        private bool[,] Collidable;

        private Texture textureMap = new Texture("maps/spritemap.png");
        //contains target size for quads
        private Vector2f idealQuadSize, currentQuadSize;
        //vertex drawing stuff
        private VertexArray tileMap;
        private RenderStates renderStates = RenderStates.Default;
        private const int EARTH_OFFSET = 0, SAND_OFFSET = 100, WATER_OFFSET_ONE = 200, WATER_OFFSET_TWO = 300;

        public TileEngine(RenderWindow _w, Vector2u _tileAmount, string _levelLocation)
        {
            win = _w;
            tileAmount = _tileAmount;
            tileTypes = new int[tileAmount.X, tileAmount.Y];
            Collidable = new bool[tileAmount.X, tileAmount.Y];

            //load texture for tilemap
            renderStates.Texture = textureMap;

            //create array of all vertices
            tileMap = new VertexArray(PrimitiveType.Quads, tileAmount.X * tileAmount.Y * 4);

            //randomly set size
            idealQuadSize = new Vector2f(64, 64);

            //initialize the current quad size
            currentQuadSize = idealQuadSize;

            //fill the tile types
            fillTileTypeArray(_levelLocation);

            //draw vertexicesarrays
            for (uint y = 0; y < tileAmount.Y; y++){
                for (uint x = 0; x < tileAmount.X; x++){
                    //because: 4 vertexes/quad * (current y times how many x per view) * x
                    uint currentPosition = 4 * ((y * tileAmount.X) + x);
                    //control textures
                    float texOffset = 0;
                    int tileType = tileTypes[x, y];
                    switch (tileType)
                    {
                        case CGlobal.SAND_TILE:
                            if (y < tileAmount.Y)
                                Collidable[x, y] = true;
                            texOffset = SAND_OFFSET;
                            break;
                        case CGlobal.WATER_TILE:
                            if (y < tileAmount.Y)
                                Collidable[x, y] = true;
                            texOffset = WATER_OFFSET_ONE;
                            break;
                            //default to earth
                        default: 
                        case CGlobal.EARTH_TILE:
                            if (y < tileAmount.Y)
                                Collidable[x, y] = true;
                            texOffset = EARTH_OFFSET;
                            break;
                    }
                    //map vertex positions
                    /* not isometric
                    tileMap[currentPosition + 0] = new Vertex(new Vector2f(idealQuadSize.X * x, idealQuadSize.Y * y), new Vector2f(texOffset, 0));//top left vertex
                    tileMap[currentPosition + 1] = new Vertex(new Vector2f(idealQuadSize.X * (x + 1), idealQuadSize.Y * y), new Vector2f(texOffset + 100, 0));//top right vertex
                    tileMap[currentPosition + 2] = new Vertex(new Vector2f(idealQuadSize.X * (x + 1), idealQuadSize.Y * (y + 1)), new Vector2f(texOffset + 100, 100));//bot right vertex
                    tileMap[currentPosition + 3] = new Vertex(new Vector2f(idealQuadSize.X * x, idealQuadSize.Y * (y + 1)), new Vector2f(texOffset + 0, 100));//bot left vertex
                     */
                    /*
                    tileMap[currentPosition + 0] = new Vertex(new Vector2f(idealQuadSize.X * x, idealQuadSize.Y * (y + 0.5f) ), new Vector2f(texOffset, 0));//top left vertex
                    tileMap[currentPosition + 1] = new Vertex(new Vector2f(idealQuadSize.X * (x + .5f), idealQuadSize.Y * y), new Vector2f(texOffset + 100, 0));//top right vertex
                    tileMap[currentPosition + 2] = new Vertex(new Vector2f(idealQuadSize.X * (x + 1), idealQuadSize.Y * (y + .5f)), new Vector2f(texOffset + 100, 100));//bot right vertex
                    tileMap[currentPosition + 3] = new Vertex(new Vector2f(idealQuadSize.X * (x + .5f), idealQuadSize.Y * (y + 1)), new Vector2f(texOffset + 0, 100));//bot left vertex
                     * 
                                         tileMap[currentPosition + 0] = new Vertex(new Vector2f(idealQuadSize.X * (0.0f + .5f * x), idealQuadSize.Y * (0.5f + .5f * y)), new Vector2f(texOffset, 0));//top left vertex
                    tileMap[currentPosition + 1] = new Vertex(new Vector2f(idealQuadSize.X * (0.5f + .5f * x), idealQuadSize.Y * (0.0f + .5f * y)), new Vector2f(texOffset + 100, 0));//top right vertex
                    tileMap[currentPosition + 2] = new Vertex(new Vector2f(idealQuadSize.X * (1.0f + .5f * x), idealQuadSize.Y * (0.5f + .5f * y)), new Vector2f(texOffset + 100, 100));//bot right vertex
                    tileMap[currentPosition + 3] = new Vertex(new Vector2f(idealQuadSize.X * (0.5f + .5f * x), idealQuadSize.Y * (1.0f + .5f * y)), new Vector2f(texOffset + 0, 100));//bot left vertex
                    */
                    //0+.5*x, .5f-y*05f
                    //.5+ .5f*x, 0-y*.5f
                    //1+.5*x, 0.5-y*.5f
                    //.5+.5x, 1-y*.5f
                    tileMap[currentPosition + 0] = new Vertex(new Vector2f(idealQuadSize.X * (0.0f + .5f * x), idealQuadSize.Y * (0.3f + .3f * -x) + idealQuadSize.Y * y * 0.6f), new Vector2f(texOffset, 0));//top left vertex
                    tileMap[currentPosition + 1] = new Vertex(new Vector2f(idealQuadSize.X * (0.5f + .5f * x), idealQuadSize.Y * (0.0f + .3f * -x) + idealQuadSize.Y * y * 0.6f), new Vector2f(texOffset + 100, 0));//top right vertex
                    tileMap[currentPosition + 2] = new Vertex(new Vector2f(idealQuadSize.X * (1.0f + .5f * x), idealQuadSize.Y * (0.3f + .3f * -x) + idealQuadSize.Y * y * 0.6f), new Vector2f(texOffset + 100, 100));//bot right vertex
                    tileMap[currentPosition + 3] = new Vertex(new Vector2f(idealQuadSize.X * (0.5f + .5f * x), idealQuadSize.Y * (0.6f + .3f * -x) + idealQuadSize.Y * y * 0.6f), new Vector2f(texOffset + 0, 100));//bot left vertex
                }
            }
        }

        /// <summary>
        /// Returns the x/y tiles (not screen coordinates) with the given tile type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Vector2u> TilesOfType(int type)
        {
            List<Vector2u> returnList = new List<Vector2u>();
            for (uint y = 0; y < tileAmount.Y; y++)
            {
                for (uint x = 0; x < tileAmount.X; x++)
                {
                    //because: 4 vertexes/quad * (current y times how many x per view) * x
                    uint currentPosition = 4 * ((y * tileAmount.X) + x);
                    if (tileTypes[x, y] == type)
                        returnList.Add(new Vector2u(x, y));
                }
            }
            return returnList;
        }

        /// <summary>
        /// Returns whether the given tile collides (see GetCurrentTile)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool GetTileCollidable(int x, int y)
        {
            return Collidable[x, y];
        }

        /// <summary>
        /// returns the rectangle of the vertex at position x, y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private FloatRect GetRectangle(int x, int y)
        {
            return new FloatRect(currentQuadSize.X * x, currentQuadSize.Y * y,
                currentQuadSize.X, currentQuadSize.Y);
        }

        public Vector2f GetXY(int x, int y)
        {
            return new Vector2f(currentQuadSize.X * x, currentQuadSize.Y * y);
        }

        /// <summary>
        /// fills tile type array using maps created by ogmo
        /// </summary>
        /// <param name="levelLocation"></param>
        private void fillTileTypeArray(string levelLocation)
        {
            //great. get tiles from a f*ckin xml file...
            XDocument xDoc = XDocument.Load(levelLocation);
            var query = xDoc.Descendants("level").Select(s => new
            {
                EARTH = s.Element("earth").Value,
                SAND = s.Element("sand").Value,
                WATER = s.Element("water").Value
            }).FirstOrDefault();
            //get strings from array, removing all linebreaks
            string earth = query.EARTH.Replace("\n", String.Empty);
            string sand = query.SAND.Replace("\n", String.Empty);
            string water = query.WATER.Replace("\n", String.Empty);
            //get byte arrays from strings
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] earthArray = enc.GetBytes(earth);
            byte[] sandArray = enc.GetBytes(sand);
            byte[] waterArray = enc.GetBytes(water);
            //get tiles from byte arrays
            //somewhat dirty implementation, we can only define tile priority
            for (int y = 0; y < tileAmount.Y; y++){
                for (int x = 0; x < tileAmount.X; x++){
                    long oneDimensionalArrayPosition = y * tileAmount.X + x;
                    //default to earth tile
                    tileTypes[x, y] = CGlobal.EARTH_TILE;

                    //decide on tiles with higher priority, low to high
                    if (sandArray[oneDimensionalArrayPosition] == 49)//earth top
                        tileTypes[x, y] = CGlobal.SAND_TILE;
                    if (waterArray[oneDimensionalArrayPosition] == 49)//ASCII one: lava
                        tileTypes[x, y] = CGlobal.WATER_TILE;
                }
            }
        }

        /// <summary>
        /// Use this to check for collisions with the tiles that are set to collidable.
        /// It returns true if the Rectangle created using the position and and size you
        /// gave intersects with any tile.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool Collides(Vector2f position, Vector2f size)
        {
            //create floatrect from input
            FloatRect aRect = new FloatRect(position.X, position.Y, size.X, size.Y);
            //check intersection for each tile
            for (int y = 0; y < tileAmount.Y; y++)
                for (int x = 0; x < tileAmount.X; x++)
                    //check each rectangles' position
                    if (aRect.Intersects(GetRectangle(x, y)) && Collidable[x, y])
                        return true;
            return false;
        }

        public bool Collides(FloatRect aRect)
        {
            //check intersection for each tile
            for (int y = 0; y < tileAmount.Y; y++)
                for (int x = 0; x < tileAmount.X; x++)
                    //check each rectangles' position
                    if (aRect.Intersects(GetRectangle(x, y)) && Collidable[x, y])
                        return true;
            return false;
        }

        /// <summary>
        /// returns the x and y grid position your given center position lies in
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        public int[] GetCurrentTile(Vector2f center)
        {
            for (int y = 0; y < tileAmount.Y; y++)
            {
                for (int x = 0; x < tileAmount.X; x++)
                {
                    //check each rectangles' position
                    if (GetRectangle(x, y).Contains(center.X, center.Y))
                        return new int[] { x, y };
                }
            }
            return new int[] { -1, -1 };
        }

        public void UpdateWindowResize()
        {
            Client.cView.Size = new Vector2f(win.Size.X, win.Size.Y);
        }

        public void Update()
        {
            return;
            //get x we are at
            uint xOffset = (uint)(CGlobal.CURRENT_WINDOW_ORIGIN.X / currentQuadSize.X);
            Color lavaColor = new Color(207, 128, 16);
            for (uint y = 0; y < tileAmount.Y; y++)
            {
                for (uint x = xOffset; x < tileAmount.X; x++)
                {
                    //if (tileTypes[x, y] == CGlobal.LAVA_TOP_TILE || tileTypes[x, y] == CGlobal.LAVATILE)
                        //lightEngine.AddLight(new Vector2f(CGlobal.CURRENT_WINDOW_ORIGIN.X + ((x - xOffset) * currentQuadSize.X), y * currentQuadSize.Y), currentQuadSize, new Vector2f(1.5f, 1.5f), lavaColor);
                }
            }
        }


        public void AnimationUpdate(){
            for (uint y = 0; y < tileAmount.Y; y++){
                for (uint x = 0; x < tileAmount.X; x++){
                    //because: 4 vertexes/quad * (current y times how many x per view) * x
                    uint currentPosition = 4 * ((y * tileAmount.X) + x);
                    //initialize updated texture coords
                    Vector2f texCo1, texCo2, texCo3, texCo4;
                    
                    int tileType = tileTypes[x, y];
                    //only water tiles are animated; only update their vertices
                    if (tileType == CGlobal.WATER_TILE){
                        Vector2f newOffset = new Vector2f(0, 0);

                        //get current texture (aka the texture offset of the tile)
                        texCo1 = tileMap[currentPosition + 0].TexCoords;
                        //swap offset / texture
                        if (texCo1.X == WATER_OFFSET_ONE)
                            newOffset.X = WATER_OFFSET_TWO - WATER_OFFSET_ONE;
                        else
                            newOffset.X = WATER_OFFSET_ONE - WATER_OFFSET_TWO;

                        //write new coordinates
                        texCo1 = tileMap[currentPosition + 0].TexCoords + newOffset;
                        texCo2 = tileMap[currentPosition + 1].TexCoords + newOffset;
                        texCo3 = tileMap[currentPosition + 2].TexCoords + newOffset;
                        texCo4 = tileMap[currentPosition + 3].TexCoords + newOffset;

                        //update vertex texture coordinates
                        tileMap[currentPosition + 0] = new Vertex(new Vector2f(currentQuadSize.X * x, currentQuadSize.Y * y), texCo1);//top left vertex
                        tileMap[currentPosition + 1] = new Vertex(new Vector2f(currentQuadSize.X * (x + 1), currentQuadSize.Y * y), texCo2);//top right vertex
                        tileMap[currentPosition + 2] = new Vertex(new Vector2f(currentQuadSize.X * (x + 1), currentQuadSize.Y * (y + 1)), texCo3);//bot right vertex
                        tileMap[currentPosition + 3] = new Vertex(new Vector2f(currentQuadSize.X * x, currentQuadSize.Y * (y + 1)), texCo4);//bot left vertex
                    }
                }
            }
        }

        public void Draw()
        {
            //timekeeping is commented out
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            tileMap.Draw(win, renderStates);
            //sw.Stop();
            //Console.WriteLine("tilemap rendering took " + sw.Elapsed.Duration()+" ms");
        }
    }
}
