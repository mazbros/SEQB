using System.Collections;
using System.Collections.Generic;

namespace SEQB
{
    public class ProofOfShipment : IEnumerable
    {
        public string ShipDate { get; set; }
        public string TrackingNumber { get; set; }
        public string PartNumber { get; set; }
        public string Freight { get; set; }
        public string Service { get; set; }
        public string PackageReference { get; set; }

        public IList<string> GetAllFields()
        {
            return new List<string>{ ShipDate, PartNumber, TrackingNumber, Freight, PackageReference, Service };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) getEnumerator();
        }

        private ProofOfShipment getEnumerator()
        {
            return new ProofOfShipment();
        }
    }
}
