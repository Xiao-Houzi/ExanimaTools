using System.Collections.Generic;
using System.Linq;

namespace ExanimaTools.Models
{
    /// <summary>
    /// Represents the user's arsenal of equipment pieces.
    /// </summary>
    public class Arsenal
    {
        // Change: Use a List<int> to allow duplicates
        private readonly List<int> _equipmentIds = new();
        private readonly List<EquipmentPiece> _equipment = new();

        /// <summary>
        /// All equipment currently in the arsenal.
        /// </summary>
        public IReadOnlyList<EquipmentPiece> Equipment => _equipment;

        /// <summary>
        /// Adds an equipment piece to the arsenal. Always adds (allows duplicates).
        /// </summary>
        public bool AddEquipment(EquipmentPiece piece)
        {
            if (piece == null)
                return false;
            _equipment.Add(piece);
            _equipmentIds.Add(piece.Id);
            return true;
        }

        /// <summary>
        /// Removes a single instance of an equipment piece by ID. Returns true if removed.
        /// </summary>
        public bool RemoveEquipment(int id)
        {
            var idx = _equipment.FindIndex(e => e.Id == id);
            if (idx == -1) return false;
            _equipmentIds.Remove(id); // Only removes one instance
            _equipment.RemoveAt(idx);
            return true;
        }

        /// <summary>
        /// Checks if the arsenal contains an equipment piece by ID.
        /// </summary>
        public bool Contains(int id) => _equipmentIds.Contains(id);

        /// <summary>
        /// Finds equipment by name (case-insensitive, partial match).
        /// </summary>
        public IEnumerable<EquipmentPiece> SearchByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return _equipment;
            return _equipment.Where(e => e.Name.Contains(name, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
