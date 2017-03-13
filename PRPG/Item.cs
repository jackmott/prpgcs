
namespace PRPG {
    public class Item {
        public int count;
        public string name;

        public Item(int count, string name) {
            this.count = count;
            this.name = name;
        }

        public override bool Equals(object obj) {
            return name.Equals(obj);
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }
    }
}
