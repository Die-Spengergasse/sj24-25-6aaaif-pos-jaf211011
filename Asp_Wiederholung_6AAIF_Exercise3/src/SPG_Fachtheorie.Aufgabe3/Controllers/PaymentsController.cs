﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe3.Commands;
using SPG_Fachtheorie.Aufgabe3.Dtos;

namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    [Route("api/[controller]")]  // --> /api/payments
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly AppointmentContext _db;

        public PaymentsController(AppointmentContext db)
        {
            _db = db;
        }

        /// <summary>
        /// GET /api/payments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<PaymentDto>> GetAllPayments(
            [FromQuery] int? cashDesk, [FromQuery] DateTime? dateFrom)
        {
            var payments = _db.Payments
                .Where(p =>
                    cashDesk.HasValue
                        ? p.CashDesk.Number == cashDesk.Value : true)
                .Where(p =>
                    dateFrom.HasValue
                        ? p.PaymentDateTime >= dateFrom.Value : true)
                .Select(p => new PaymentDto(
                    p.Id, p.Employee.FirstName, p.Employee.LastName,
                    p.PaymentDateTime,
                    p.CashDesk.Number, p.PaymentType.ToString(),
                    p.PaymentItems.Sum(p => p.Amount)))
                .ToList();
            return Ok(payments);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PaymentDetailDto> GetPaymentDetail(int id)
        {
            var payment = _db.Payments
                .Where(p => p.Id == id)
                .Select(p => new PaymentDetailDto(
                    p.Id, p.Employee.FirstName, p.Employee.LastName,
                    p.CashDesk.Number, p.PaymentType.ToString(),
                    p.PaymentItems
                        .Select(pi => new PaymentItemDto(
                            pi.ArticleName, pi.Amount, pi.Price))
                        .ToList()
                    ))
                .FirstOrDefault();
            if (payment is null)
                return NotFound();
            return Ok(payment);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreatePayment(NewPaymentCommand cmd)
        {
            var employee = _db.Employees.FirstOrDefault(e => e.RegistrationNumber == cmd.EmployeeRegistrationNumber);
            if (employee is null)
                return BadRequest("Employee not found");
            var cashDesk = _db.CashDesks.FirstOrDefault(cd => cd.Number == cmd.CashDeskNumber);
            if (cashDesk is null)
                return BadRequest("Cash desk not found");
            var payment = new Payment(cashDesk,
                cmd.PaymentDateTime,
                employee,
                Enum.Parse<PaymentType>(cmd.PaymentType)
            );
            _db.Payments.Add(payment);
            try
            {
                _db.SaveChanges();  // INSERT INTO
            }
            catch (DbUpdateException e)
            {
                return Problem(
                    e.InnerException?.Message ?? e.Message,
                    statusCode: StatusCodes.Status400BadRequest);
            }
            return CreatedAtAction(nameof(CreatePayment), new { payment.Id });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeletePayment(int id, [FromQuery] bool deleteItems)
        {
            if (!deleteItems) return BadRequest("deleteItems must be true");
            var payment = _db.Payments.FirstOrDefault(p => p.Id == id);
            if (payment is null)
                return NoContent();
            _db.Payments.Remove(payment);
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return Problem(
                    e.InnerException?.Message ?? e.Message,
                    statusCode: StatusCodes.Status400BadRequest);
            }
            catch (InvalidOperationException e)
            {
                return Problem(
                    e.InnerException?.Message ?? e.Message,
                    statusCode: StatusCodes.Status400BadRequest);
            }
            return NoContent();
        }

        [HttpPut("/api/paymentItems/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePaymentItem(int id, UpdatePaymentItemCommand cmd)
        {
            if (id != cmd.Id)
                return Problem("Invalid payment item ID", statusCode: 400);
            var paymentItem = _db.PaymentItems.FirstOrDefault(p => p.Id == id);
            if (paymentItem is null)
                return Problem("Payment Item not found", statusCode: 404);
            var payment = _db.Payments.FirstOrDefault(p => p.Id == id);
            if (payment is null)
                return Problem("Payment not found", statusCode: 400);
            if (paymentItem.LastUpdated != cmd.LastUpdated)
                return Problem("Payment Item has changed", statusCode: 400);
            paymentItem.ArticleName = cmd.ArticleName;
            paymentItem.Amount = cmd.Amount;
            paymentItem.Price = cmd.Price;
            paymentItem.LastUpdated = DateTime.UtcNow;
            try
            {
                _db.SaveChanges();
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Message = "One or more required arguments are missing", Error = ex.Message });
            }
            catch (DbUpdateException e)
            {
                return Problem(e.InnerException?.Message ?? e.Message);
            }
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateConfirmed (int id)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == id);
            if (payment is null)
                return Problem("Payment not Found", statusCode: 404);
            if (payment.Confirmed is not null)
                return Problem("Payment already confirmed", statusCode: 400);
            payment.Confirmed = payment.Confirmed;
            return SaveorThrow();
            return NoContent();
        }

        private IActionResult SaveorThrow()
        {
            try
            {
                _db.SaveChanges();
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Message = "One or more required arguments are missing", Error = ex.Message });
            }
            catch (DbUpdateException e)
            {
                return Problem(e.InnerException?.Message ?? e.Message);
            }
        }
    }
}