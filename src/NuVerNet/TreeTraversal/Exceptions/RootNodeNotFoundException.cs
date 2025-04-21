using NuVerNet.Abstract;

namespace NuVerNet.TreeTraversal.Exceptions;

public class RootNodeNotFoundException() : AbstractException("Root node is null");