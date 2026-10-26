using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ExportFileDto
{
    public string FileName { get; set; }
        = string.Empty;

    public byte[] FileBytes { get; set; }
        = Array.Empty<byte>();
}