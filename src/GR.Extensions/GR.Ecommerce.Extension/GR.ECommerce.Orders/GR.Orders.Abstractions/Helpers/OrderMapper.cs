using System;
using System.Collections.Generic;
using System.Linq;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Models;
using GR.Orders.Abstractions.Models;

namespace GR.Orders.Abstractions.Helpers
{
    public static class OrderMapper
    {
        /// <summary>
        /// Map cart to order
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static Order Map(Cart cart, string notes)
        {
            if (cart == null) throw new NullReferenceException();
            var order = new Order
            {
                UserId = cart.UserId,
                Notes = notes,
                OrderState = OrderState.New
            };

            var orderItems = cart.CartItems.Select(x =>
            {
                var pOrder = new ProductOrder
                {
                    ProductVariationId = x.ProductVariationId,
                    Amount = x.Amount,
                    DiscountValue = x.Product?.DiscountPrice ?? 0,
                    OrderId = order.Id,
                    ProductId = x.ProductId
                };
                if (x.ProductVariation == null)
                {
                    pOrder.PriceWithOutDiscount = x.Product?.PriceWithoutDiscount ?? 0;
                }
                else
                {
                    pOrder.PriceWithOutDiscount = x.ProductVariation.Price;
                }

                return pOrder;
            });

            order.ProductOrders = orderItems.ToList();

            return order;
        }

        /// <summary>
        /// Map product to order
        /// </summary>
        /// <param name="product"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Order Map(Product product, int amount = 1)
        {
            var order = new Order
            {
                OrderState = OrderState.New,
                ProductOrders = new List<ProductOrder>
                {
                    new ProductOrder
                    {
                        ProductId = product.Id,
                        Amount = amount,
                        PriceWithOutDiscount = product.PriceWithoutDiscount,
                        DiscountValue = product.DiscountPrice,
                    }
                }
            };

            return order;
        }

        /// <summary>
        /// Map variation product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="variation"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Order Map(Product product, ProductVariation variation, int amount = 1)
        {
            if (product == null) throw new NullReferenceException();
            var order = new Order
            {
                OrderState = OrderState.New,
                ProductOrders = new List<ProductOrder>
                {
                    new ProductOrder
                    {
                        ProductId = product.Id,
                        Amount = amount,
                        PriceWithOutDiscount = variation == null ? product.PriceWithoutDiscount : variation.Price,
                        DiscountValue = product.DiscountPrice,
                        ProductVariationId = variation?.Id
                    }
                }
            };

            return order;
        }
    }
}
