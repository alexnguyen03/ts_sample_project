using BlazorBootstrap;

using ClientProject.Components.Models;

using Microsoft.AspNetCore.Components;

using Newtonsoft.Json;

using ServerProject.Models;

using ZXingBlazor.Components;
namespace ClientProject.Components.Pages
{
    public partial class POS
    {
        [Inject]
        HttpClient httpClient { get; set; }

        BarcodeReader? barcodeReader;
        bool ShowScanBarcode { get; set; } = true;
        Employee? employee { get; set; }
        public string? BarCode { get; set; }
        public int SelectedUnitId { get; set; }
        public Product? foundProduct;
        public bool Pdf417 { get; set; }
        public bool DecodeContinuously { get; set; }
        public bool DecodeAllFormats { get; set; }
        private string message { get; set; } = "";
        public List<ProductInOrder>? listProductInOrder { get; set; }
        List<ToastMessage> messages = new List<ToastMessage>();
        private List<Customer>? listCustomers { get; set; }
        public string? CustomerId { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            employee ??= new();
            employee.LastName = "Hoài Nam";
            employee.EmployeeId = 1;
            foundProduct = new Product();
            listProductInOrder = new List<ProductInOrder>();
            listCustomers = new List<Customer>();
            GetAllCustomer();
        }
        private void ScanResult(string e)
        {
            if (!DecodeContinuously)
            {
                BarCode = e;
                //ShowScanBarcode = !ShowScanBarcode;
                try
                {
                    int.Parse(e);
                    GetProductById(int.Parse(e));
                }
                catch (Exception ex)
                {
                    message = "Sản phẩm không đúng(không tồn tại trong kho)";
                    Console.WriteLine(ex);
                    return;
                }
            }
            else
            {
                if (BarCode.Length > 200) BarCode = "";
                BarCode += e + Environment.NewLine;
            }
        }

        private Task OnError(string message)
        {
            this.message = message;
            StateHasChanged();
            return Task.CompletedTask;
        }
        public async Task GetProductById(int id)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Product/getProduct?ProductId={id}");
                if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    var errorMessage = response.ReasonPhrase;
                    Console.WriteLine($"There was an error! {errorMessage}");
                    message = "Sản phẩm không đúng(không tồn tại trong kho)";
                    return;
                }
                string responseData = await response.Content.ReadAsStringAsync();
                foundProduct = JsonConvert.DeserializeObject<Product>(responseData)!;
                var existingProduct = listProductInOrder.FirstOrDefault(p => p.ProductId == foundProduct!.ProductId)!;
                if (existingProduct != null)
                {
                    existingProduct.Quantity++;
                }
                else
                {
                    ProductInOrder productInOrder = new ProductInOrder();
                    productInOrder.ProductId = foundProduct.ProductId;
                    productInOrder.ProductName = foundProduct.ProductName;
                    productInOrder.Unit = foundProduct.Units.FirstOrDefault()!;
                    productInOrder.Units = foundProduct.Units;
                    productInOrder.Quantity = 1;
                    listProductInOrder.Add(productInOrder);
                }
                message = "";
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex);
            }

        }
        private void HandleUnitSelection(int productId, string unitId)
        {
            var selectedProduct = listProductInOrder!.Find(pr => pr.ProductId == productId);
            if (selectedProduct != null)
            {
                var selectedUnit = selectedProduct.Units.ToList().Find(u => u.UnitId == int.Parse(unitId));
                if (selectedUnit != null)
                {
                    selectedProduct.Unit = selectedUnit;
                }
                StateHasChanged();
            }
        }
        private void ShowMessage(ToastType toastType) => messages.Add(CreateToastMessage(toastType));
        private ToastMessage CreateToastMessage(ToastType toastType)
  => new ToastMessage
  {
      Type = toastType,
      Message = $"Thanh toán thành công!",
  };
        private void UpdateTotalPrice(string value, int prdId)
        {
            ProductInOrder item = listProductInOrder.Find(prd => prd.ProductId == prdId)!;
            if (item != null)
            {
                item.Quantity = int.Parse(value);
                StateHasChanged();
            }
        }
        private Double DisplayTotalPrice()
        {
            return listProductInOrder.Sum(prd =>
            {
                return Convert.ToDouble(prd.Quantity) * Convert.ToDouble(prd.Unit.UnitPrice);
            });
        }
        private void RemoveProduct(int productId)
        {
            var prodToRemove = listProductInOrder.Single(r => r.ProductId == productId);
            listProductInOrder.Remove(prodToRemove);
        }
        private async Task GetAllCustomer()
        {

            try
            {
                var response = await httpClient.GetAsync("/api/Customer/getCustomersWithoutPage");
                if (!response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    var errorMessage = response.ReasonPhrase;
                    Console.WriteLine($"GetAllCustomer:  There was an error ! {errorMessage}");
                    return;
                }
                string responseData = await response.Content.ReadAsStringAsync();
                listCustomers = JsonConvert.DeserializeObject<List<Customer>>(responseData)!;
                StateHasChanged();
            }
            catch (Exception ex) { await Console.Out.WriteLineAsync("Error: " + ex); }


        }

        private async Task Payment()
        {
            Pos newPos = new Pos();
            newPos.CustomerId = CustomerId;
            newPos.TotalPrice = DisplayTotalPrice();
            newPos.EmployeeId = employee.EmployeeId;
            List<PosDetail> listPosDetail = new List<PosDetail>();
            foreach (var item in listProductInOrder!)
            {
                PosDetail posDetail = new PosDetail();
                posDetail.ProductId = item.ProductId;
                posDetail.TotalPrice = (double?)(item.Quantity * item.Unit.UnitPrice);
                posDetail.PricePerUnit = (double?)item.Unit.UnitPrice;
                posDetail.BatchNumber = "TRUNGSON PHARMA";
                posDetail.Quantity = item.Quantity;
                posDetail.UnitName = item.Unit.UnitName;
                listPosDetail.Add(posDetail);
            }
            newPos.PosDetails = listPosDetail;
            var response = await httpClient.PostAsJsonAsync("api/Pos/createPos", newPos);
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = response.ReasonPhrase;
                Console.WriteLine($"PostAsJsonAsync in POS UI! {errorMessage}");
                return;
            }
            ShowMessage(ToastType.Success);
        }

    }
}
