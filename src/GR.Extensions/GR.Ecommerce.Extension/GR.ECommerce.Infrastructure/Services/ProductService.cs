using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using GR.Cache.Abstractions.Extensions;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Validators;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.Models.Currencies;
using GR.ECommerce.Abstractions.Models.Settings;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.Identity.Abstractions;
using GR.MultiTenant.Abstractions.Helpers;
using GR.Orders.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Infrastructure.Services
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [Documentation("Basic Implementation of product service")]
    public class ProductService : IProductService<Product>
    {
        #region Injectable

        /// <summary>
        /// Inject commerce context
        /// </summary>
        public ICommerceContext Context { get; }

        /// <summary>
        /// Inject payment context
        /// </summary>
        private readonly IPaymentContext _paymentContext;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject order db context
        /// </summary>
        private readonly IOrderDbContext _orderDbContext;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        public ProductService(ICommerceContext commerceContext, IUserManager<GearUser> userManager, IPaymentContext paymentContext, IOrderDbContext orderDbContext, IMapper mapper)
        {
            Context = commerceContext;
            _userManager = userManager;
            _paymentContext = paymentContext;
            _orderDbContext = orderDbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Product>>> GetAllProducts(Func<Product, bool> predicate = null)
        {
            var result = new ResultModel<IEnumerable<Product>>();
            var data = await Context.Products.ToListAsync();
            if (predicate != null)
            {
                data = data.Where(predicate).ToList();
            }

            result.IsSuccess = true;
            result.Result = data;
            return result;
        }

        /// <summary>
        /// Get products with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<ProductsPaginatedViewModel>> GetProductsWithPaginationAsync(DTParameters parameters)
        {
            var paginated = await Context.Products.GetPagedAsDtResultAsync(parameters);
            return _mapper.Map<DTResult<ProductsPaginatedViewModel>>(paginated);
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Product>> GetProductByIdAsync(Guid? productId)
        {
            if (productId == null) return new NotFoundResultModel<Product>();
            var product = await Context.Products
                .Include(x => x.Brand)
                .Include(x => x.ProductAttributes)
                .ThenInclude(x => x.ProductAttribute)
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductVariations)
                .ThenInclude(x => x.ProductVariationDetails)
                .ThenInclude(x => x.ProductOption)
                .FirstOrDefaultAsync(x => x.Id.Equals(productId));
            if (product == null) return new NotFoundResultModel<Product>();
            var response = new ResultModel<Product>
            {
                IsSuccess = true,
                Result = product
            };

            return response;
        }

        /// <summary>
        /// Add product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddProductAsync([Required] Product product)
        {
            if (product == null) return new InvalidParametersResultModel<Guid>();
            var modelState = ModelValidator.IsValid(product);
            if (!modelState.IsSuccess) return modelState.Map<Guid>();
            await Context.Products.AddAsync(product);
            var dbResult = await Context.PushAsync();
            return dbResult.Map(product.Id);
        }

        /// <summary>
        /// get product by min number value by attribute name 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public async Task<ResultModel<Product>> GetProductByAttributeMinNumberValueAsync(string attribute)
        {
            var valueInt = 0;
            var listAttribute = Context.ProductAttributes
                .Include(i => i.ProductAttribute)
                .Include(i => i.Product)
                .ThenInclude(i => i.ProductAttributes)
                .Where(x => x.ProductAttribute.Name == attribute && int.TryParse(x.Value, out valueInt));

            if (!listAttribute.Any())
            {
                return new NotFoundResultModel<Product>();
            }

            var minValue = listAttribute.Min(x => int.Parse(x.Value));
            var product = (await listAttribute.FirstOrDefaultAsync(x => int.Parse(x.Value) == minValue))?.Product;

            var resultModel = new ResultModel<Product>
            {
                IsSuccess = true,
                Result = product
            };

            return resultModel;
        }

        #region ProductTypes

        /// <summary>
        /// Add new product type
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddProductTypeAsync(ProductType productType)
        {
            if (productType == null) return new InvalidParametersResultModel<Guid>();
            var modelState = ModelValidator.IsValid(productType);
            if (!modelState.IsSuccess) return modelState.Map<Guid>();
            await Context.ProductTypes.AddAsync(productType);
            var dbResponse = await Context.PushAsync();
            return dbResponse.Map(productType.Id);
        }

        #endregion

        #region Settings

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<Currency>> GetGlobalCurrencyAsync()
        {
            var settings = await Context.CommerceSettings
                .FirstOrDefaultAsync(x => x.Key.Equals(CommerceResources.SettingsParameters.CURRENCY));

            if (settings == null) return await SetGlobalCurrencyAsync(CommerceResources.SystemCurrencies.USD);
            {
                var currency =
                    await Context.Currencies.FirstOrDefaultAsync(x => x.Code.Equals(settings.Value));
                return new SuccessResultModel<Currency>(currency);
            }

        }

        /// <summary>
        /// Set global currency for products
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ResultModel<Currency>> SetGlobalCurrencyAsync(string code)
        {
            var currency = await Context.Currencies
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                x.Code.Equals(code));

            if (currency == null) return new NotFoundResultModel<Currency>();
            var dbResponse = await AddOrUpdateSettingAsync(CommerceResources.SettingsParameters.CURRENCY, currency.Code);
            return dbResponse.Map(currency);
        }

        /// <summary>
        /// Get all currencies
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Currency>>> GetAllCurrenciesAsync() =>
            new SuccessResultModel<IEnumerable<Currency>>(await Context.Currencies.FromCache(TimeSpan.MaxValue).ToListAsync());

        /// <summary>
        /// Get setting
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultModel<TOutput>> GetSettingAsync<TOutput>(string key)
            where TOutput : class
        {
            var setting = await Context.CommerceSettings.FirstOrDefaultAsync(x => x.Key.Equals(key));
            if (setting == null) return new InvalidParametersResultModel<TOutput>();
            return new SuccessResultModel<TOutput>(setting.Value.Deserialize<TOutput>());
        }

        /// <summary>
        /// Add or update setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddOrUpdateSettingAsync(string key, object value, CommerceSettingType type = CommerceSettingType.Text)
        {
            var setting = await Context.CommerceSettings.FirstOrDefaultAsync(x => x.Key.Equals(key));
            if (setting == null)
            {
                await Context.CommerceSettings.AddAsync(new CommerceSetting
                {
                    Key = key,
                    Value = ParseSettingValue(value, type)
                });
            }
            else
            {
                setting.Value = ParseSettingValue(value, type);
                Context.CommerceSettings.Update(setting);
            }

            return await Context.PushAsync();
        }

        #endregion

        #region Filters

        /// <summary>
        /// Get products by filters
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Product>>> GetProductsWithFiltersAsync([Required] ProductsFilterRequest model)
        {
            var modelState = ModelValidator.IsValid(model);
            if (!modelState.IsSuccess) return modelState.Map<IEnumerable<Product>>();
            var response = new ResultModel<IEnumerable<Product>>();
            var queryProducts = Context.Products
                .Include(x => x.Brand)
                .Include(x => x.ProductAttributes)
                .ThenInclude(x => x.ProductAttribute)
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductVariations)
                .ThenInclude(x => x.ProductVariationDetails)
                .ThenInclude(x => x.ProductOption)
                .AsQueryable();

            foreach (var filter in model.Filters ?? new List<CommerceFilter>())
            {
                switch (filter.ParameterName)
                {
                    case "SearchGlobal":
                        {
                            Expression<Func<Product, bool>> globalSearch = x => x.Name.Contains(filter.ParameterValue) || x.DisplayName.Contains(filter.ParameterValue);
                            queryProducts = queryProducts.Where(globalSearch);
                        }
                        break;
                    case "Categories":
                        {
                            var categories = filter.ParameterValue.Deserialize<IEnumerable<CommerceFilter>>();
                            if (categories == null) continue;
                            var catIds = categories.Where(x => !x.ParameterValue.IsNullOrEmpty()).Select(x => x.ParameterValue.ToGuid())
                                .ToList();
                            foreach (var id in catIds)
                            {
                                Expression<Func<Product, bool>> categoryFilter = x => x.ProductCategories.Select(i => i.CategoryId).Contains(id);
                                queryProducts = queryProducts.Where(categoryFilter);
                            }
                        }
                        break;
                    case "Brands":
                        {
                            var brands = filter.ParameterValue.Deserialize<IEnumerable<CommerceFilter>>();
                            if (brands == null) continue;
                            var bIds = brands.Where(x => !x.ParameterValue.IsNullOrEmpty()).Select(x => x.ParameterValue.ToGuid())
                                .ToList();
                            if (!bIds.Any()) continue;
                            Func<Product, List<Guid>, bool> bFilter = (p, bds) => bds.Contains(p.BrandId);
                            queryProducts = queryProducts.Where(t => bFilter(t, bIds));
                        }
                        break;
                    case "Attributes":
                        {
                            var attributes = filter.ParameterValue.Deserialize<IEnumerable<CommerceFilter>>().ToList();
                            if (!attributes.Any()) continue;
                            queryProducts = queryProducts.Where(p => p.ProductAttributes.Any(x => attributes.Select(m => m.ParameterName).Contains(x.Value))
                                                                     && p.ProductAttributes.Any(j => attributes.Select(m => StringExtensions.Split(m.ParameterValue, "_")[1].ToGuid())
                                                                         .Contains(j.ProductAttributeId)));
                        }
                        break;
                }
            }



            var products = await queryProducts
               //.Skip((model.Page - 1) * model.PerPage).Take(model.PerPage)
               .ToListAsync();

            response.IsSuccess = true;
            response.Result = products;
            return response;
        }

        #endregion

        #region Statistic

        /// <summary>
        /// Get commerce general statistic
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ResultModel<SalesStatisticViewModel>> GetCommerceGeneralStatisticAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null) return new InvalidParametersResultModel<SalesStatisticViewModel>();
            var model = new SalesStatisticViewModel
            {
                NewCustomers = (await GetNewCustomersGeneralInfoAsync(startDate, endDate)).Result,
                TotalEarnings = (await GetTotalEarningsAsync(startDate, endDate)).Result,
                OrderReceived = (await GetOrderReceivedStatisticAsync(startDate, endDate)).Result
            };
            return new SuccessResultModel<SalesStatisticViewModel>(model);
        }

        /// <summary>
        /// Get year report
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<Dictionary<int, object>>> GetYearReportAsync()
        {
            var year = DateTime.Today.Year;
            var data = new Dictionary<int, object>();
            for (var i = 1; i <= 12; i++)
            {
                var users = await _userManager.UserManager.Users.CountAsync(x => x.Created.Year.Equals(year) && x.Created.Month.Equals(i));
                var orders = await _orderDbContext.Orders.Where(x => x.OrderState == OrderState.PaymentReceived)
                    .CountAsync(x => x.Created.Year.Equals(year) && x.Created.Month.Equals(i));
                data.Add(i, new
                {
                    Users = users,
                    Sales = orders
                });
            }
            return new SuccessResultModel<Dictionary<int, object>>(data);
        }

        #endregion

        #region Product Variations

        /// <summary>
        /// Get price by variation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> GetPriceByVariationAsync(ProductPriceVariationViewModel model)
        {
            var modelState = ModelValidator.IsValid(model);
            if (!modelState.IsSuccess) return modelState;
            var resultModel = new ResultModel();
            var prod = await Context.Products
                .NonDeleted()
                .Include(i => i.ProductPrices)
                .FirstOrDefaultAsync(x => x.Id == model.ProductId);

            if (prod != null)
            {
                if (model.VariationId is null)
                {
                    resultModel.IsSuccess = true;
                    resultModel.Result = new { Price = prod.PriceWithDiscount * model.Quantity };
                    return resultModel;
                }

                var productVariation = Context.ProductVariations
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == model.VariationId);

                if (productVariation is null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                    return resultModel;
                }

                resultModel.IsSuccess = true;
                resultModel.Result = new
                {
                    Price = productVariation.Price * model.Quantity
                };
                return resultModel;
            }

            resultModel.IsSuccess = false;
            resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));


            return resultModel;
        }

        /// <summary>
        /// Remove variation options
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variationId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveVariationOptionAsync([Required] Guid? productId, [Required] Guid? variationId)
        {
            var resultModel = new ResultModel();
            if (productId == null || variationId == null) return new InvalidParametersResultModel();

            var result = Context.ProductVariations
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == variationId && x.ProductId == productId);

            if (result == null) return resultModel;
            Context.ProductVariations.Remove(result);
            var dbResult = await Context.PushAsync();
            return dbResult;
        }

        /// <summary>
        /// Add or update variation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddOrUpdateVariationAsync(UpdateProductVariationViewModel model)
        {
            var validate = ModelValidator.IsValid(model);
            if (!validate.IsSuccess) return validate;
            var prod = await Context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == model.ProductId);

            if (prod == null) return new NotFoundResultModel();

            var variation = await Context.ProductVariations
                .FirstOrDefaultAsync(i => i.Id == model.VariationId && i.ProductId == model.ProductId);

            if (variation != null)
            {
                variation.Price = model.Price;
                Context.ProductVariations.Update(variation);

                var listProductVariationDetails =
                   Context.ProductVariationDetails.Where(x => x.ProductVariationId == variation.Id);

                Context.ProductVariationDetails.RemoveRange(listProductVariationDetails);
            }
            else
            {
                variation = new ProductVariation
                {
                    ProductId = model.ProductId,
                    Price = model.Price,
                };

                await Context.ProductVariations.AddAsync(variation);
            }

            var variationDetails = model.ProductVariationDetails.Select(x => new ProductVariationDetail()
            {
                ProductVariationId = variation.Id,
                Value = x.Value,
                ProductOptionId = x.ProductOptionId
            });

            await Context.ProductVariationDetails.AddRangeAsync(variationDetails);


            var dbResult = await Context.PushAsync();
            return dbResult;
        }

        /// <summary>
        /// Get product variations
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<ProductVariationViewModel>>> GetProductVariationsAsync(Guid productId)
        {
            var variations = await Context.ProductVariations
                .Include(x => x.ProductVariationDetails).ThenInclude(x => x.ProductOption)
                .Where(x => x.ProductId == productId)
                .Select(x => new ProductVariationViewModel
                {
                    VariationId = x.Id,
                    Price = x.Price,
                    VariationDetails = x.ProductVariationDetails.Select(s => new ProductVariationDetailsViewModel
                    {
                        Value = s.Value,
                        Option = s.ProductOption.Name,
                        OptionId = s.ProductOptionId
                    })
                }).ToListAsync();

            return new SuccessResultModel<IEnumerable<ProductVariationViewModel>>(variations);
        }

        /// <summary>
        /// Get product variation by id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variationId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<ProductVariationViewModel>> GetProductVariationByIdAsync(Guid productId, Guid variationId)
        {
            if (productId == Guid.Empty || variationId == Guid.Empty) return new InvalidParametersResultModel<ProductVariationViewModel>();
            var variation = await Context
                .ProductVariations
                .Include(x => x.ProductVariationDetails).ThenInclude(x => x.ProductOption)
                .Where(x => x.ProductId == productId && x.Id == variationId)
                .Select(x => new ProductVariationViewModel
                {
                    ProductId = x.ProductId,
                    VariationId = x.Id,
                    Price = x.Price,
                    VariationDetails = x.ProductVariationDetails.Select(s => new ProductVariationDetailsViewModel
                    {
                        Value = s.Value,
                        Option = s.ProductOption.Name,
                        OptionId = s.ProductOptionId
                    })
                }).FirstOrDefaultAsync();
            if (variation == null) return new NotFoundResultModel<ProductVariationViewModel>();

            return new SuccessResultModel<ProductVariationViewModel>(variation);
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Parse setting
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string ParseSettingValue(object value, CommerceSettingType type)
        {
            var response = string.Empty;

            switch (type)
            {
                case CommerceSettingType.Text:
                case CommerceSettingType.Number:
                    response = value.ToString();
                    break;
                case CommerceSettingType.Object:
                case CommerceSettingType.Array:
                    response = value.SerializeAsJson();
                    break;
            }

            return response;
        }

        /// <summary>
        /// Get order statistic
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task<ResultModel<OrderReceivedStatisticViewModel>> GetOrderReceivedStatisticAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null) return new InvalidParametersResultModel<OrderReceivedStatisticViewModel>();
            var total = await _orderDbContext.Orders.CountAsync();
            var inPeriod = await _orderDbContext.Orders
                .AsNoTracking()
                .CountAsync(x => x.Created >= startDate && x.Created <= endDate);
            var model = new OrderReceivedStatisticViewModel
            {
                TotalOrderReceived = inPeriod,
                Percentage = inPeriod.PercentOf(total)
            };
            return new SuccessResultModel<OrderReceivedStatisticViewModel>(model);
        }

        /// <summary>
        /// Get total earnings info
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task<ResultModel<TotalEarningsStatisticViewModel>> GetTotalEarningsAsync(
            DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null) return new InvalidParametersResultModel<TotalEarningsStatisticViewModel>();
            var total = await _paymentContext.Payments
                .Where(x => x.PaymentStatus.Equals(PaymentStatus.Succeeded))
                .ToListAsync();
            var inPeriod = await _paymentContext.Payments
                .Where(x => x.PaymentStatus.Equals(PaymentStatus.Succeeded) && x.Created >= startDate &&
                            x.Created <= endDate)
                .ToListAsync();
            var model = new TotalEarningsStatisticViewModel
            {
                TotalEarnings = inPeriod?.Sum(x => x.Total) ?? 0,
                Percentage = inPeriod?.Count.PercentOf(total.Count) ?? 0
            };
            return new SuccessResultModel<TotalEarningsStatisticViewModel>(model);
        }

        /// <summary>
        /// Get customers info
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task<ResultModel<NewCustomersStatisticViewModel>> GetNewCustomersGeneralInfoAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null) return new InvalidParametersResultModel<NewCustomersStatisticViewModel>();
            var companyRole = await _userManager.RoleManager.FindByNameAsync(MultiTenantResources.Roles.COMPANY_ADMINISTRATOR);
            if (companyRole == null) return new NotFoundResultModel<NewCustomersStatisticViewModel>();
            var newCustomersRequest = await _userManager.GetUsersInRolesAsync(new List<GearRole> { companyRole });
            if (!newCustomersRequest.IsSuccess) return newCustomersRequest.Map<NewCustomersStatisticViewModel>();
            var newCustomers = newCustomersRequest.Result.ToList();
            var inPeriod = newCustomers.Where(x => x.Created >= startDate && x.Created <= endDate).ToList();
            var model = new NewCustomersStatisticViewModel
            {
                NewCustomers = inPeriod.Count,
                Percentage = inPeriod.Count.PercentOf(newCustomers.Count)
            };
            return new SuccessResultModel<NewCustomersStatisticViewModel>(model);
        }

        #endregion
    }
}