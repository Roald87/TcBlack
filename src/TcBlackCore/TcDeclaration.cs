namespace TcBlackCore
{
    public struct TcDeclaration
    {
        public TcDeclaration(
            string name,
            string allocation,
            string dataType,
            string initialization,
            string comment
        )
        {
            Name = name;
            Allocation = allocation;
            DataType = dataType;
            Initialization = initialization;
            Comment = comment;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (TcDeclaration)obj;
            return 
                Name == other.Name 
                && Allocation == other.Allocation 
                && DataType == other.DataType
                && Initialization == other.Initialization 
                && Comment == other.Comment
            ;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator==(TcDeclaration obj1, TcDeclaration obj2)
        {
            if (obj1 == null)
            {
                return obj2 == null;
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(TcDeclaration obj1, TcDeclaration obj2)
        {
            return !(obj1 == obj2);
        }

        public string Name { get; }
        public string Allocation { get; }
        public string DataType { get; }
        public string Initialization { get; }
        public string Comment { get; }
    }
}