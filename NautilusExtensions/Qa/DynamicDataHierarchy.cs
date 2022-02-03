using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NautilusExtensions.Qa {
    public class Sample {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public char Status { get; set; }
        public string PartNumber { get; set; }
        public string SerialNumber { get; set; }
        public string ProgramCode { get; set; }
        public string ExternalReference { get; set; }
        public string ReceiverNumber { get; set; }  //stores file name of file used when uploading
        public string TreeNodeName { get { return string.Format("{0} ({1} - {2})", Name, PartNumber, SerialNumber); } }
        public List<Aliquot> Aliquots { get; set; }

        public int ProgenyCount {
            get {
                int count = Aliquots.Count;
                foreach (Aliquot a in Aliquots) {
                    count += a.Tests.Count;
                    foreach (Test t in a.Tests) {
                        count += t.Results.Count;
                    }
                }
                return count;
            }
        }

        public Sample() {
            Name = "New Sample";
            Status = 'n';
            ExternalReference = System.Guid.NewGuid().ToString();
            Aliquots = new List<Aliquot>();
        }
    }

    public class Aliquot {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExternalReference { get; set; }
        public char Status { get; set; }
        public string TreeNodeName { get { return Name; } }
        public List<Test> Tests { get; set; }

        public Aliquot() {
            Name = "New Aliquot";
            Status = 'n';
            ExternalReference = System.Guid.NewGuid().ToString();
            Tests = new List<Test>();
        }
    }

    public class Test {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExternalReference { get; set; }
        public char Status { get; set; }
        public string MatrixId { get; set; }
        public string IpItemNumber { get; set; }
        public string RowId { get; set; }
        public string TreeNodeName { get { return $"{Name} ({MatrixId}, {IpItemNumber}, {RowId})"; } }
        public List<Result> Results { get; set; }

        public Test() {
            Name = "New Test";
            Status = 'n';
            ExternalReference = System.Guid.NewGuid().ToString();
            Results = new List<Result>();
        }
    }

    public class Result {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRowId { get; set; }
        public string Template { get { return IsRowId ? "Row ID text" : "Text"; } }
        public string ExternalReference { get; set; }
        public string Description { get; set; }
        public char Status { get; set; }
        public string ResultValue { get; set; }
        public string TreeNodeName { get { return Name + " = " + ResultValue; } }

        public Result() {
            Name = "New Result";
            Status = 'n';
            ExternalReference = System.Guid.NewGuid().ToString();
        }
    }
}
