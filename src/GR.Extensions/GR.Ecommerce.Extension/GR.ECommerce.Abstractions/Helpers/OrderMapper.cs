﻿using System;
using System.Linq;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Models;

namespace GR.ECommerce.Abstractions.Helpers
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
                    Price = x.Product?.PriceWithDiscount ?? 0,
                    OrderId = order.Id,
                    ProductId = x.ProductId
                };
                return pOrder;
            });

            order.ProductOrders = orderItems.ToList();

            return order;
        }
    }
}
