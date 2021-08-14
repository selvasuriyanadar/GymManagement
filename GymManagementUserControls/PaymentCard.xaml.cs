using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GymManagementDataModel;
using GymManagementHILogic;

namespace GymManagementUserControls
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class PaymentCard : UserControl
  {
    public PaymentCard(PaymentDetails td)
    {
      InitializeComponent();

      SelectionString.Text = td.SelectionAsString();
      DateOfPayment.Text = ((DateTime) td.payment_data.date_of_payment).ToString("dd/MM/yyyy");
      Amount.Text = td.payment_data.amount.ToString();
      PaymentStatus.Content = PaymentData.PaymentStatusDict[(PaymentData.PaymentStatus) td.payment_data.payment_status];
      switch (td.payment_data.payment_status)
      {
        case PaymentData.PaymentStatus.Paid:
          PaymentStatus.SetResourceReference(Control.ForegroundProperty, "PrimaryHueMidBrush");
          break;
        case PaymentData.PaymentStatus.NotPaid:
          PaymentStatus.SetResourceReference(Control.ForegroundProperty, "SecondaryHueMidBrush");
          break;
        case PaymentData.PaymentStatus.Lent:
          PaymentStatus.SetResourceReference(Control.ForegroundProperty, "SecondaryHueDarkBrush");
          break;
        case PaymentData.PaymentStatus.LentReturned:
          PaymentStatus.SetResourceReference(Control.ForegroundProperty, "PrimaryHueMidBrush");
          break;
      }
    }

    public PaymentCard()
    {
      InitializeComponent();

      SelectionString.Text = "aaaaaaaaa";
      DateOfPayment.Text = "aa/aa/aa";
      Amount.Text = "aa";
      PaymentStatus.Content = "aaaa";
      PaymentStatus.SetResourceReference(Control.ForegroundProperty, "PrimaryHueMidBrush");
    }
  }
}
