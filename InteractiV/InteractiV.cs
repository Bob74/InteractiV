using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GTA;
using GTA.Native;
using GTA.Math;

using static InteractiV.InteractiProp;

/*
    
    slash tires 

 */


namespace InteractiV
{
    public class InteractiV : Script
    {
        private List<InteractiProp> propsList;
        public InteractiV()
        {
            propsList = new List<InteractiProp>();
            propsList.AddRange(LoadInteractiProps());

            Tick += OnTick;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
        }


        // Dispose Event
        protected override void Dispose(bool A_0)
        {
            if (A_0)
            {

            }
        }

        // OnTick Event
        void OnTick(object sender, EventArgs e)
        {
            float dist = 1.5f;
            Vector3 playerPos = Game.Player.Character.Position;
            Vehicle slashVehicle = World.GetClosestVehicle(playerPos, 2.0f);

            if (slashVehicle != null)
                if (slashVehicle.Exists())
                    if (slashVehicle.CanTiresBurst)
                        if (Game.IsControlJustReleased(0, GTA.Control.Context))
                            slashVehicle.BurstTire(Tools.Vehicles.GetClosestTire(slashVehicle));
                



            foreach (InteractiProp prop in propsList)
            {
                object ingameObj = Function.Call<object>(Hash.GET_CLOSEST_OBJECT_OF_TYPE, playerPos.X, playerPos.Y, playerPos.Z, dist, Game.GenerateHash(prop.ModelName), false, false, false);
                Prop ingameProp = (Prop)ingameObj;

                if (ingameProp != null)
                    if (ingameProp.Exists())
                    {
                        // Check if accessibility flags are respected
                        if (IsInteractiPropAccessible(prop))
                            continue;
                        
                        // Display HelpText
                        if (prop.HelpText != "")
                            Tools.UI.DisplayHelpTextThisFrame(prop.HelpText + ingameProp.Position.ToString());

                        // Display Marker
                        //
                        
                        if (Game.IsControlJustReleased(0, prop.ControlInput))
                            ExecuteAction(prop.Action);
                        
                        break;
                    }
            }
        }

        // KeyDown Event
        internal void OnKeyDown(object sender, KeyEventArgs e)
        { }

        // KeyUp Event
        internal void OnKeyUp(object sender, KeyEventArgs e)
        { }


        private void ExecuteAction(Action action)
        {

        }

    }
}
