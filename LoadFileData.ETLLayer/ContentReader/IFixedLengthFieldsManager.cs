namespace LoadFileData.ETLLayer.ContentReader
{
    public interface IFixedLengthFieldsManager
    {
        int[] GetFieldWidths();

        void AddField(string fieldName, int width);
    }
}
