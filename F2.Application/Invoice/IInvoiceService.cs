using F2.Application.Invoice.Dtos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Invoice
{
    public interface IInvoiceService
    {
       InvoiceResponse InvoiceCallBack(int timestamp, object dto);
    }
}
