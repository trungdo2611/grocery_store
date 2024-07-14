using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using WebQLyNongSan.Models;
using WebQLyNongSan.Models.PaymentOnline;

namespace WebQLyNongSan.Controllers
{
    public class ShoppingCartController : Controller
    {
        QLyNongSanEntities2 db = new QLyNongSanEntities2();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            if (TempData["msgSuccess"] != null)
            {
                ViewBag.msgSuccess = TempData["msgSuccess"].ToString();
                TempData["msgSuccess"] = null;
            }
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return View(cart.Items);
            }
            return View();
        }


        public ActionResult CheckOut()
        {
            if(Session["userId"] == null) {
                return RedirectToAction("Index", "Home");
            }
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return View(cart.Items);
            }
            return View();
        }

        public ActionResult CheckOutSuccess()
        {
            if (TempData["successOrder"] != null)
            {
                ViewBag.successOrder = TempData["successOrder"].ToString();
                TempData["successOrder"] = null;
            }
            return View();
        }

        [HttpPost]
        public ActionResult CheckOut(OrderViewModel req)
        {
                var code = new { Success = false, Code = -1, Url = "" };
                int userId = (int)Session["userId"];
                if (ModelState.IsValid)
                {
                    ShoppingCart cart = (ShoppingCart)Session["Cart"];
                    if (cart != null)
                    {
                        OrderTable order = new OrderTable();
                        order.FullName = req.FullName;
                        order.Email = req.Email;
                        order.Address = req.Address;
                        order.PaymentType = req.PaymentType;
                        order.Phone = req.Phone;
                        order.Create_at = DateTime.Now;
                        order.UserID = userId;
                        order.StransactStatusId = 1;

                        foreach (var item in cart.Items)
                        {
                            Product product = db.Products.Find(item.id);
                            if (product != null)
                            {
                                if (item.Quantity > product.UnitStock)
                                {
                                    ModelState.AddModelError("Quantity", "Sản phẩm không có đủ số lượng.");
                                    return View(req); // Trả lại view với ModelState chứa thông báo lỗi
                                }
                                product.UnitStock -= item.Quantity;
                            }

                            order.OrderDetails.Add(new OrderDetail
                            {
                                ProductID = item.id,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                create_at = DateTime.Now,
                            });
                        }
                        order.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));
                        Random rd = new Random();
                        order.Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                        db.OrderTables.Add(order);
                        db.SaveChanges();

                        //Send mail cho khách hàng
                        var strSanPham = "";
                        var thanhtien = decimal.Zero;
                        foreach (var item in cart.Items)
                        {
                            strSanPham += "<tr>";
                            strSanPham += "<td>" + item.Name + "</td>";
                            strSanPham += "<td>" + item.Quantity + "</td>";
                            strSanPham += "<td>" + string.Format("{0:#,0}", item.TotalPrice) + "</td>";
                            strSanPham += "</tr>";
                            thanhtien += item.Price * item.Quantity;
                        }
                        string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/send2.html"));
                        contentCustomer = contentCustomer.Replace("{{MaDon}}", order.Code);
                        contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                        contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                        contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.FullName);
                        contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
                        contentCustomer = contentCustomer.Replace("{{DiaChi}}", order.Address);
                        contentCustomer = contentCustomer.Replace("{{Email}}", order.Email);
                        contentCustomer = contentCustomer.Replace("{{ThanhTien}}", string.Format("{0:#,0}", thanhtien));
                        WebQLyNongSan.Models.Common.SendMail("GroCo_ShopOnline", "Đơn hàng #" + order.Code, contentCustomer.ToString(), req.Email);
                        TempData["successOrder"] = "Bạn đã đặt đơn hàng thành công!!";
                        cart.ClearCart();
                        code = new { Success = true, Code = req.PaymentType, Url = "" };
                        if (req.PaymentType == 2)
                        {
                            var url = UrlPayment(req.PaymentTypeVN, order.Code);
                            code = new { Success = true, Code = req.PaymentType, Url = url };
                        }
                        //return RedirectToAction("CheckOutSuccess");
                    }
                }

                return Json(code);
            
        }

        public ActionResult Partial_CheckOut()
        {

            return PartialView();
        }


        public ActionResult PartialCart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];

            if (cart != null)
            {
                return PartialView("PartialCart", cart.Items);
            }
            return PartialView();
        }

        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            var checkProduct = db.Products
                        .Where(x => x.id == id)
                        .Join(db.Categories,
                         product => product.CategoryId,
                         category => category.id,
                        (product, category) => new { Product = product, Category = category })
    .FirstOrDefault();
            if (checkProduct != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                ShoppingCartItem item = new ShoppingCartItem()
                {
                    id = checkProduct.Product.id,
                    Name = checkProduct.Product.Name,
                    Quantity = quantity,
                    CategoryName = checkProduct.Category.Name,
                };
                item.Picture = checkProduct.Product.Picture;
                item.Price = (decimal)checkProduct.Product.Price;
                item.TotalPrice = item.Quantity * item.Price;
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart;
                code = new { Success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công!!", code = 1, Count = cart.Items.Count() };

            }
            return Json(code);
        }

        [HttpPost]
        public ActionResult Update(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.id == id);
                if (checkProduct != null)
                {
                    cart.UpdateQuantity(id, quantity);
                    TempData["msgSuccess"] = "Bạn đã cập nhật thành công sản phẩm này";
                    return Json(new { Success = true });
                }


            }
            return Json(new { Success = false });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.id == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
        }

        public ActionResult VnpayReturn()
        {
            if (Request.QueryString.Count > 0)
            {
                if (TempData["successOrder"] != null)
                {
                    ViewBag.successOrder = TempData["successOrder"].ToString();
                    TempData["successOrder"] = null;
                }
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = db.OrderTables.FirstOrDefault(x => x.Code == orderCode);
                        if (itemOrder != null)
                        {
                            itemOrder.StransactStatusId = 3;//đã thanh toán
                            db.OrderTables.Attach(itemOrder);
                            db.Entry(itemOrder).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        //Thanh toan thanh cong
                        ViewBag.InnerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                       
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        //log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    ViewBag.ThanhToanThanhCong = "Số tiền thanh toán " + vnp_Amount.ToString() + " VNĐ";
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
            }
            //var a = UrlPayment(0, "DH3574");
            return View();
        }

        #region Thanh toán vnpay
        public string UrlPayment(int TypePaymentVN, string orderCode)
        {
            var urlPayment = "";
            var order = db.OrderTables.FirstOrDefault(x => x.Code == orderCode);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TotalAmount * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.Code);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Code); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion
    }
}