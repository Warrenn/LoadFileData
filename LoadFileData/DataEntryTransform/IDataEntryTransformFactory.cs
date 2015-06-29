using System;

namespace LoadFileData.DataEntryTransform
{
    public interface IDataEntryTransformFactory
    {
        IDataEntryTransform CreateTransform(Guid typeGuid);
    }
}
