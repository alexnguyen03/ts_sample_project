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
        HttpClient? HttpClient { get; set; }

        BarcodeReader? barcodeReader;
        bool ShowScanBarcode { get; set; } = true;
        Employee? Employee { get; set; }
        public string? BarCode { get; set; }
        public int SelectedUnitId { get; set; }
        public Product? foundProduct;
        public bool Pdf417 { get; set; }
        public bool DecodeContinuously { get; set; }
        public bool DecodeAllFormats { get; set; }
        private string? Message { get; set; }
        public List<ProductInOrder>? ListProductInOrder { get; set; }
        List<ToastMessage> messages = new List<ToastMessage>();
        private List<Customer>? ListCustomers { get; set; }
        public string? CustomerId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Employee ??= new();
            Employee.LastName = "Hoài Nam";
            Employee.EmployeeId = 1;
            foundProduct = new Product();
            ListProductInOrder = new List<ProductInOrder>();
            ListCustomers = new List<Customer>();
            await GetAllCustomer();
        }

        private async void ScanResult(string e)
        {
            if (!DecodeContinuously)
            {
                BarCode = e;
                //ShowScanBarcode = !ShowScanBarcode;
                try
                {
                    _ = int.Parse(e);
                    await GetProductById(int.Parse(e));
                }
                catch (Exception ex)
                {
                    Message = "Sản phẩm không đúng(không tồn tại trong kho)";
                    Console.WriteLine(ex);
                    return;
                }
            }
            else
            {
                if (BarCode!.Length > 200)
                {
                    BarCode = "";
                }

                BarCode += e + Environment.NewLine;
            }
        }

        private Task OnError(string message)
        {
            this.Message = message;
            StateHasChanged();
            return Task.CompletedTask;
        }

        public async Task GetProductById(int id)
        {
            try
            {
                var response = await HttpClient!.GetAsync($"api/Product/getProduct?ProductId={id}");
                if (
                    !response.IsSuccessStatusCode
                    || response.StatusCode == System.Net.HttpStatusCode.NoContent
                )
                {
                    var errorMessage = response.ReasonPhrase;
                    Console.WriteLine($"There was an error! {errorMessage}");
                    Message = "Sản phẩm không đúng(không tồn tại trong kho)";
                    return;
                }
                string responseData = await response.Content.ReadAsStringAsync();
                foundProduct = JsonConvert.DeserializeObject<Product>(responseData)!;
                var existingProduct = ListProductInOrder!.FirstOrDefault(p =>
                    p.ProductId == foundProduct!.ProductId
                )!;
                if (existingProduct != null)
                {
                    existingProduct.Quantity++;
                }
                else
                {

                    ProductInOrder productInOrder = new();
                    productInOrder.ProductId = foundProduct.ProductId;
                    productInOrder.ProductName = foundProduct.ProductName;
                    productInOrder.Unit = foundProduct.Units.FirstOrDefault()!;
                    productInOrder.Units = foundProduct.Units;

                    productInOrder.Quantity = 1;
                    ListProductInOrder!.Add(productInOrder);
                }
                Message = "";
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex);
            }
        }

        private void HandleUnitSelection(int productId, string unitId)
        {
            var selectedProduct = ListProductInOrder!.Find(pr => pr.ProductId == productId);
            if (selectedProduct != null)
            {
                var selectedUnit = selectedProduct
                    .Units.ToList()
                    .Find(u => u.UnitId == int.Parse(unitId));
                if (selectedUnit != null)
                {
                    selectedProduct.Unit = selectedUnit;
                }
                StateHasChanged();
            }
        }

        private void ShowMessage(ToastType toastType) =>
            messages.Add(CreateToastMessage(toastType));

        private ToastMessage CreateToastMessage(ToastType toastType) =>
            new()
            { Type = toastType, Message = $"Thanh toán thành công!", };

        private void UpdateTotalPrice(string value, int prdId)
        {
            ProductInOrder item = ListProductInOrder!.Find(prd => prd.ProductId == prdId)!;
            if (item != null)
            {
                item.Quantity = int.Parse(value);
                StateHasChanged();
            }
        }

        private Double DisplayTotalPrice()
        {
            return ListProductInOrder!.Sum(prd =>
            {
                return Convert.ToDouble(prd.Quantity) * Convert.ToDouble(prd.Unit.UnitPrice);
            });
        }

        private void RemoveProduct(int productId)
        {
            var prodToRemove = ListProductInOrder!.Single(r => r.ProductId == productId);
            ListProductInOrder!.Remove(prodToRemove);
        }

        private async Task GetAllCustomer()
        {
            try
            {
                var response = await HttpClient!.GetAsync("api/Customer/getCustomersWithoutPage");
                if (
                    !response.IsSuccessStatusCode
                    || response.StatusCode == System.Net.HttpStatusCode.NoContent
                )
                {
                    var errorMessage = response.ReasonPhrase;
                    Console.WriteLine($"GetAllCustomer:  There was an error ! {errorMessage}");
                    return;
                }
                string responseData = await response.Content.ReadAsStringAsync();
                ListCustomers = JsonConvert.DeserializeObject<List<Customer>>(responseData)!;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi chuyển tiếp: {ex.Message}");
            }
        }

        private async Task Payment()
        {
            Pos newPos = new Pos();
            newPos.CustomerId = CustomerId;
            newPos.TotalPrice = DisplayTotalPrice();
            newPos.EmployeeId = Employee!.EmployeeId;
            List<PosDetail> listPosDetail = new List<PosDetail>();
            foreach (var item in ListProductInOrder!)
            {
                PosDetail posDetail = new();
                posDetail.ProductId = item.ProductId;
                posDetail.TotalPrice = (double?)(item.Quantity * item.Unit.UnitPrice);
                posDetail.PricePerUnit = (double?)item.Unit.UnitPrice;
                posDetail.BatchNumber = "TRUNGSON PHARMA";
                posDetail.Quantity = item.Quantity;
                posDetail.UnitName = item.Unit.UnitName;
                listPosDetail.Add(posDetail);
            }
            newPos.PosDetails = listPosDetail;
            var response = await HttpClient!.PostAsJsonAsync("api/Pos/createPos", newPos);
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
