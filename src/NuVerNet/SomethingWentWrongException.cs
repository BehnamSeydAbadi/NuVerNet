using NuVerNet.Abstract;

namespace NuVerNet;

public class SomethingWentWrongException(string value) : AbstractException(value);