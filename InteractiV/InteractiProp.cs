using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


using GTA;
using GTA.Math;



/*
<?xml version="1.0" encoding="UTF-8"?>
<props>
	<prop>
		<modelName>prop_table_03_chr</modelName>
		<action>"SIT"</action>
		<controlInput>INPUT_CONTEXT</controlInput>
		<accessibility>0x1</accessibility>
		<marker>0</marker>
		<helpText>Appuyez sur ~INPUT_CONTEXT~ pour vous asseoir.</helpText>
		<offsets>
			<offset>
				<x>0.0f</x>
				<y>0.0f</y>
				<z>-0.5f</z>
			</offset>
		</offsets>
	</prop>
</props> 
*/

namespace InteractiV
{
    class InteractiProp
    {
        private string _modelName;
        public string ModelName {
            get { return _modelName; }
            private set { _modelName = value; }
        }

        private Action _action;
        public Action Action {
            get { return _action; }
            private set { _action = value; }
        }

        List<Vector3> _offsets = new List<Vector3>();
        public List<Vector3> Offsets {
            get { return _offsets; }
            private set { _offsets.AddRange(value); }
        }

        private Control _controlInput;
        public Control ControlInput
        {
            get { return _controlInput; }
            private set { _controlInput = value; }
        }

        private Accessibility _accessibilityFlags;
        public Accessibility AccessibilityFlags {
            get { return _accessibilityFlags; }
            private set { _accessibilityFlags = value; }
        }

        private Marker _markerType = Marker.None;
        public Marker MarkerType {
            get { return _markerType; }
            private set { _markerType = value; }
        }

        private string _helpText = "";
        public string HelpText {
            get { return _helpText; }
            private set { _helpText = value; }
        }

        public InteractiProp(string name, Action action, List<Vector3> offsets, Control control = Control.Context, Accessibility access = Accessibility.All, Marker marker = Marker.None, string helpText = "")
        {
            ModelName = name;
            Action = action;
            Offsets = offsets;
            ControlInput = control;
            AccessibilityFlags = access;
            MarkerType = marker;
            HelpText = helpText;
        }


        // STATIC METHODS
        private static string _propsFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\InteractiV\\propsList.xml";

        public static List<InteractiProp> LoadInteractiProps()
        {
            List<InteractiProp> propsList = new List<InteractiProp>();

            if (System.IO.File.Exists(_propsFilePath))
            {
                XElement file = XElement.Load(_propsFilePath);

                if (file.Element("prop") != null)
                {
                    foreach (XElement prop in file.Elements("prop"))
                    {
                        string modelName = "";
                        Action action = Action.None;
                        List<Vector3> offsets = new List<Vector3>();
                        Control controlInput = Control.Context;
                        Accessibility accessibilityFlags = Accessibility.None;
                        Marker markerType = Marker.None;
                        string helpText = "";

                        XElement section = prop.Element("modelName");
                        if (section != null)
                            modelName = section.Value;

                        section = prop.Element("action");
                        if (section != null)
                            action = (Action)Enum.Parse(typeof(Action), section.Value);

                        section = prop.Element("offsets");
                        if (section != null)
                        {
                            if (section.Element("offset") != null)
                            {
                                foreach (XElement offset in section.Elements("offset"))
                                {
                                    float x, y, z;
                                    if (!float.TryParse(offset.Element("x").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out x))
                                        x = 0f;
                                    if (!float.TryParse(offset.Element("y").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out y));
                                        y = 0f;
                                    if (!float.TryParse(offset.Element("z").Value, NumberStyles.Any, CultureInfo.InvariantCulture, out z));
                                        z = 0f;

                                    offsets.Add(new Vector3(x, y, z));
                                }
                            }
                        }

                        section = prop.Element("control");
                        if (section != null)
                            controlInput = (Control)Enum.Parse(typeof(Control), section.Value);

                        section = prop.Element("accessibility");
                        if (section != null)
                            accessibilityFlags = (Accessibility)Enum.Parse(typeof(Accessibility), section.Value);

                        section = prop.Element("marker");
                        if (section != null)
                            markerType = (Marker)Enum.Parse(typeof(Marker), section.Value);

                        section = prop.Element("helpText");
                        if (section != null)
                            helpText = section.Value;

                        // Add the prop to the list
                        propsList.Add(new InteractiProp(modelName, action, offsets, controlInput, accessibilityFlags, markerType, helpText));
                    }
                }
            }
            else
                Logger.Log("Error: LoadInteractiProps - The file doesn't exist " + _propsFilePath);

            return propsList;
        }

        public static bool IsInteractiPropAccessible(InteractiProp prop)
        {
            bool valid = false;

            if (prop.AccessibilityFlags.HasFlag(Accessibility.OnFoot))
                if (!Game.Player.Character.IsInVehicle()) valid = true;
            if (prop.AccessibilityFlags.HasFlag(Accessibility.InVehicle))
                if (Game.Player.Character.IsInVehicle()) valid = true;
            if (prop.AccessibilityFlags.HasFlag(Accessibility.InCar))
                if (Game.Player.Character.CurrentVehicle != null)
                    if (Game.Player.Character.CurrentVehicle.Model.IsCar)
                        valid = true;
            if (prop.AccessibilityFlags.HasFlag(Accessibility.InBoat))
                if (Game.Player.Character.CurrentVehicle != null)
                    if (Game.Player.Character.CurrentVehicle.Model.IsBoat)
                        valid = true;
            if (prop.AccessibilityFlags.HasFlag(Accessibility.InPlane))
                if (Game.Player.Character.CurrentVehicle != null)
                    if (Game.Player.Character.CurrentVehicle.Model.IsPlane)
                        valid = true;
            if (prop.AccessibilityFlags.HasFlag(Accessibility.InHeli))
                if (Game.Player.Character.CurrentVehicle != null)
                    if (Game.Player.Character.CurrentVehicle.Model.IsHelicopter)
                        valid = true;
            if (prop.AccessibilityFlags.HasFlag(Accessibility.InTaxi))
                if (Game.Player.Character.CurrentVehicle != null)
                    if (Game.Player.Character.IsInTaxi)
                        valid = true;
            if (prop.AccessibilityFlags.HasFlag(Accessibility.OnBike))
                if (Game.Player.Character.CurrentVehicle != null)
                    if (Game.Player.Character.CurrentVehicle.Model.IsBike)
                        valid = true;
            if (prop.AccessibilityFlags.HasFlag(Accessibility.OnBicyle))
                if (Game.Player.Character.CurrentVehicle != null)
                    if (Game.Player.Character.CurrentVehicle.Model.IsBicycle)
                        valid = true;

            return valid;
        }








    }

    // Utiliser | pour combiner les flags.
    // Utiliser & pour détecter si le flag est présent dans la valeur actuelle (= Contains()).
    // https://stackoverflow.com/questions/8447/what-does-the-flags-enum-attribute-mean-in-c
    //
    [Flags]
    internal enum Accessibility
    {
        None = 0x0,
        OnFoot = 0x1,
        InVehicle = 0x2,
        InCar = 0x4,
        InBoat = 0x8,
        InPlane = 0x10,
        InHeli = 0x20,
        InTaxi = 0x40,
        OnBike = 0x80,
        OnBicyle = 0x100,
        All = OnFoot | InVehicle | InCar | InBoat | InPlane | InHeli | InTaxi | OnBike | OnBicyle
    }
    internal enum Marker
    {
        None = 0
    }
    internal enum Action
    {
        // Une action par animation/scenario ?
        // Ou une action par type d'action réorientée ensuite suivant le modèle de la prop ?
        None = 0,
        Sit,
        DrinkCoffee,
        DrinkEcola,
        DrinkSprunk,
    }
}
