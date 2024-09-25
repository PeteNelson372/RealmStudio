namespace RealmStudio
{
    internal class Cmd_ChangeLabelRotation : IMapOperation
    {
        private MapLabel Label;
        private float RotationValue;

        public Cmd_ChangeLabelRotation(MapLabel label, float rotationValue)
        {
            Label = label;
            RotationValue = rotationValue;
        }

        public void DoOperation()
        {
            Label.LabelRotationDegrees = RotationValue;
        }

        public void UndoOperation()
        {
            Label.LabelRotationDegrees = 0;
        }
    }
}
