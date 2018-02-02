using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using GTA;
using GTA.Native;
using GTA.Math;


    public static class Tools
    {
        /*
            Color syntax:
            ~r~ = Red, ~b~ = Blue, ~g~ = Green, ~y~ = Yellow, ~p~ = Purple, ~o~ = Orange, ~c~ = Grey, ~m~ = Darker Grey
            ~u~ = Black, ~n~ = New Line, ~s~ = Default White, ~w~ = White, ~h~ = Bold Text, ~nrt~ = ???

            Special characters:
            ¦ = Rockstar Verified Icon (U+00A6:Broken Bar - Alt+0166)
            ÷ = Rockstar Icon (U+00F7:Division Sign - Alt+0247)
            ∑ = Rockstar Icon 2 (U+2211:N-Ary Summation)
        */
        public static class Player
        {
            /// <summary>
            /// Return the current Ped type.
            /// 0 = Michael
            /// 1 = Franklin
            /// 2 = Trevor
            /// (Trevor is actually 3 but he is 2 everywhere else so if we have the value 3, we still return 2).
            /// </summary>
            /// <returns>Character's ID.</returns>
            public static int GetCurrentCharacterID()
            {
                int character = Function.Call<int>(Hash.GET_PED_TYPE, Game.Player.Character);
                if (character == 3)
                    character = 2;
                return character;
            }

            /// <summary>
            /// Return the character's name.
            /// </summary>
            /// <param name="full">Return first name and last name.</param>
            /// <returns>Character's name</returns>
            public static string GetCurrentCharacterName(bool full = false)
            {
                string lastname = "";

                switch (GetCurrentCharacterID())
                {
                    case 0:
                        if (full)
                            lastname = " De Santa";
                        return "Michael" + lastname;
                    case 1:
                        if (full)
                            lastname = " Clinton";
                        return "Franklin" + lastname;
                    case 2:
                        if (full)
                            lastname = " Philips";
                        return "Trevor" + lastname;
                    default:
                        return "???";
                }
            }

            /// <summary>
            /// Add cash to the current player. Use negative value to remove cash.
            /// </summary>
            /// <param name="value"></param>
            /// <returns>Return false if the player doesn't have enought money.</returns>
            public static bool AddCashToPlayer(int value)
            {
                int hash = Game.GenerateHash(string.Format("SP(0)_TOTAL_CASH", GetCurrentCharacterID()));

                OutputArgument outCurrentCash = new OutputArgument(0);
                Function.Call<bool>(Hash.STAT_GET_INT, hash, outCurrentCash, -1);
                int currentCash = outCurrentCash.GetResult<int>();

                if (value < 0)
                {
                    if (currentCash >= value)
                    {
                        currentCash += value;
                        Function.Call<bool>(Hash.STAT_SET_INT, hash, currentCash, 1);   // Remove cash
                        return true;
                    }
                    else return false;
                }
                else
                {
                    currentCash += value;
                    Function.Call<bool>(Hash.STAT_SET_INT, hash, currentCash, 1);       // Add cash
                    return true;
                }
            }
        }

        public static class UI
        {


            /// <summary>
            /// Display help text (Top left corner, can contain Keypress icons).
            /// ! Needs to be called every frame !
            /// </summary>
            /// <param name="text"></param>
            public static void DisplayHelpTextThisFrame(string text)
            {
                Function.Call(Hash._SET_TEXT_COMPONENT_FORMAT, "STRING");                   //  BEGIN_TEXT_COMMAND_DISPLAY_HELP
                Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);                       //  ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME
                Function.Call(Hash._DISPLAY_HELP_TEXT_FROM_STRING_LABEL, 0, 0, 1, -1);      //  END_TEXT_COMMAND_DISPLAY_HELP
            }

            /// <summary>
            /// Display a notification.
            /// </summary>
            /// <param name="text"></param>
            public static void DrawNotification(string text)
            {
                Function.Call(Hash._SET_NOTIFICATION_TEXT_ENTRY, "CELL_EMAIL_BCON");
                Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, "CELL_EMAIL_BCON");          //  ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME
                Function.Call(Hash._DRAW_NOTIFICATION, false, true);
            }
            /// <summary>
            /// Display a notification with a picture.
            /// </summary>
            /// <param name="picture"></param>
            /// <param name="title"></param>
            /// <param name="subtitle"></param>
            public static void DrawNotification(string picture, string title, string subtitle, string message)
            {
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, picture, false);
                while (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, picture))
                    Script.Yield();

                Function.Call(Hash._SET_NOTIFICATION_TEXT_ENTRY, "STRING");
                Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, message);
                Function.Call(Hash._SET_NOTIFICATION_MESSAGE, picture, picture, false, 4, title, subtitle);
                Function.Call(Hash._DRAW_NOTIFICATION, false, true);
            }


            /// <summary>
            /// Display a text at the bottom of the screen (mission like)
            /// ! Needs to be called every frame !
            /// </summary>
            /// <param name="text"></param>
            /// <param name="font"></param>
            /// <param name="centre"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="scale"></param>
            /// <param name="r"></param>
            /// <param name="g"></param>
            /// <param name="b"></param>
            /// <param name="a"></param>
            public static void DrawText(string text, int font, bool centre, float x, float y, float scale, int r, int g, int b, int a)
            {
                Function.Call(Hash.SET_TEXT_FONT, font);
                Function.Call(Hash.SET_TEXT_PROPORTIONAL, 0);
                Function.Call(Hash.SET_TEXT_SCALE, scale, scale);
                Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, a);
                Function.Call(Hash.SET_TEXT_DROPSHADOW, 0, 0, 0, 0, 255);
                Function.Call(Hash.SET_TEXT_EDGE, 1, 0, 0, 0, 255);
                Function.Call(Hash.SET_TEXT_DROP_SHADOW);
                Function.Call(Hash.SET_TEXT_OUTLINE);
                Function.Call(Hash.SET_TEXT_CENTRE, centre);
                Function.Call(Hash._SET_TEXT_ENTRY, "STRING");
                Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
                Function.Call(Hash._DRAW_TEXT, x, y);
            }
        }

        public static class World
        {
            public static void DrawMarker(Vector3 pos)
            {
                // void DRAW_MARKER(int type, float posX, float posY, float posZ
                //  float dirX, float dirY, float dirZ, float rotX, float rotY, float rotZ,
                //  float scaleX, float scaleY, float scaleZ, 
                //  int red, int green, int blue, int alpha, 
                //  BOOL bobUpAndDown, BOOL faceCamera, int p19, BOOL rotate, char* textureDict, char* textureName, BOOL drawOnEnts)
                Color color = Color.FromArgb(128, 255, 255, 0);

                Function.Call(Hash.DRAW_MARKER, 1, pos.X, pos.Y, pos.Z,
                    0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                    2.0f, 2.0f, 0.5f,
                    color.R, color.G, color.B, color.A,
                    false, false, 2, false, 0, 0, false);
            }
            public static void DrawMarker(Vector3 pos, MarkerTypes marker, Vector3 scale)
            {
                Color color = Color.FromArgb(128, 255, 255, 0);

                Function.Call(Hash.DRAW_MARKER, (int)marker, pos.X, pos.Y, pos.Z,
                    0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                    scale.X, scale.Y, scale.Z,
                    color.R, color.G, color.B, color.A,
                    false, false, 2, false, 0, 0, false);
            }
            public static void DrawMarker(Vector3 pos, MarkerTypes marker, Vector3 scale, Color color)
            {
                Function.Call(Hash.DRAW_MARKER, (int)marker, pos.X, pos.Y, pos.Z,
                    0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                    scale.X, scale.Y, scale.Z,
                    color.R, color.G, color.B, color.A,
                    false, false, 2, false, 0, 0, false);
            }

            public enum MarkerTypes
            {
                MarkerTypeUpsideDownCone = 0,
                MarkerTypeVerticalCylinder = 1,
                MarkerTypeThickChevronUp = 2,
                MarkerTypeThinChevronUp = 3,
                MarkerTypeCheckeredFlagRect = 4,
                MarkerTypeCheckeredFlagCircle = 5,
                MarkerTypeVerticleCircle = 6,
                MarkerTypePlaneModel = 7,
                MarkerTypeLostMCDark = 8,
                MarkerTypeLostMCLight = 9,
                MarkerTypeNumber0 = 10,
                MarkerTypeNumber1 = 11,
                MarkerTypeNumber2 = 12,
                MarkerTypeNumber3 = 13,
                MarkerTypeNumber4 = 14,
                MarkerTypeNumber5 = 15,
                MarkerTypeNumber6 = 16,
                MarkerTypeNumber7 = 17,
                MarkerTypeNumber8 = 18,
                MarkerTypeNumber9 = 19,
                MarkerTypeChevronUpx1 = 20,
                MarkerTypeChevronUpx2 = 21,
                MarkerTypeChevronUpx3 = 22,
                MarkerTypeHorizontalCircleFat = 23,
                MarkerTypeReplayIcon = 24,
                MarkerTypeHorizontalCircleSkinny = 25,
                MarkerTypeHorizontalCircleSkinny_Arrow = 26,
                MarkerTypeHorizontalSplitArrowCircle = 27,
                MarkerTypeDebugSphere = 28,
                MarkerTypeDallorSign = 29,
                MarkerTypeHorizontalBars = 30,
                MarkerTypeWolfHead = 31
            };

        }

        public static class Vehicles
        {

            /// <summary>
            /// Translate the model hash into its name.
            /// ! This name is not unique ! (variations of a model can have the same name)
            /// </summary>
            /// <param name="veh"></param>
            /// <returns></returns>
            public static string GetModelName(Vehicle veh)
            {
                return Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, veh.Model.Hash);
            }
            public static string GetModelName(VehicleHash hash)
            {
                return Function.Call<string>(Hash.GET_DISPLAY_NAME_FROM_VEHICLE_MODEL, (int)hash);
            }

            /// <summary>
            /// Spawns a copy of a vehicle.
            /// Useful to copy a dead vehicle.
            /// </summary>
            /// <param name="oldVeh"></param>
            public static Vehicle SpawnCopyVehicle(Vector3 coordinates, float heading, Vehicle oldVeh)
            {
                Vehicle veh = GTA.World.CreateVehicle(oldVeh.Model, coordinates, heading);

                try
                {
                    // Plate
                    veh.NumberPlate = oldVeh.NumberPlate;
                    veh.NumberPlateType = oldVeh.NumberPlateType;

                    // Mods
                    bool customTire1 = Function.Call<bool>(Hash.GET_VEHICLE_MOD_VARIATION, oldVeh, 23);
                    bool customTire2 = Function.Call<bool>(Hash.GET_VEHICLE_MOD_VARIATION, oldVeh, 24);  // Bike only

                    if (Function.Call<int>(Hash.GET_NUM_MOD_KITS, oldVeh) != 0)
                    {
                        veh.InstallModKit();

                        for (int i = 0; i <= 49; i++)
                        {
                            veh.SetMod((VehicleMod)i, oldVeh.GetMod((VehicleMod)i), false);

                            if (i == 17 || i == 18 || i == 19 || i == 20 || i == 21 || i == 22)
                                if (Function.Call<bool>(Hash.IS_TOGGLE_MOD_ON, oldVeh, i))
                                    Function.Call(Hash.TOGGLE_VEHICLE_MOD, veh, i, true);
                        }
                    }
                    veh.CanTiresBurst = oldVeh.CanTiresBurst;
                    veh.WindowTint = oldVeh.WindowTint;

                    // Tire's smoke color
                    veh.TireSmokeColor = oldVeh.TireSmokeColor;

                    // Neons
                    veh.NeonLightsColor = oldVeh.NeonLightsColor;
                    for (int i = 0; i < 4; i++)
                        veh.SetNeonLightsOn((VehicleNeonLight)i, oldVeh.IsNeonLightsOn((VehicleNeonLight)i));


                    // Color
                    veh.PrimaryColor = oldVeh.PrimaryColor;
                    veh.SecondaryColor = oldVeh.SecondaryColor;
                    veh.PearlescentColor = oldVeh.PearlescentColor;
                    veh.RimColor = oldVeh.RimColor;


                    // Convertible
                    // 0 -> up ; 1->lowering down ; 2->down ; 3->raising up
                    if (oldVeh.IsConvertible)
                        veh.RoofState = oldVeh.RoofState;

                    // Extra
                    for (int i = 1; i < 15; i++)
                    {
                        if (oldVeh.IsExtraOn(i))
                            veh.ToggleExtra(i, true);
                    }

                    // Liveries
                    veh.Livery = oldVeh.Livery;

                    // Misc
                    veh.NeedsToBeHotwired = false;
                    veh.IsStolen = false;
                }
                catch (Exception e)
                {
                    Logger.Log("Error: SpawnCopyVehicle - " + e.Message);
                }

                return veh;
            }

            public static int GetClosestTire(Vehicle veh)
            {
                
                
                return 0;
            }

        }

        public static class Camera
        {
            /// <summary>
            /// Fades the screen.
            /// </summary>
            /// <param name="duration">The time the fade should take, in milliseconds.</param>
            public static void Fade(int fadeOutDuration, int fadeInDuration = 0)
            {
                if (fadeInDuration == 0)
                    fadeInDuration = fadeOutDuration;
                Game.FadeScreenOut(fadeOutDuration);
                Script.Wait(fadeOutDuration + 500);
                Game.FadeScreenIn(fadeOutDuration);
            }
        }
    }
