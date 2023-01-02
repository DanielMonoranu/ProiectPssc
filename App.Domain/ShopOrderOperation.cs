using App.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Models.ShopOrder;
using static LanguageExt.Prelude;
using LanguageExt;
using System.Net;

namespace App.Domain
{
    public static class ShopOrderOperation
    {


        public static Task<IShopOrder> ValidateShopOrders(Func<OrderRegistrationNumber, Option<OrderRegistrationNumber>> checkOrderExists, UnvalidatedShopOrders shopOrders) =>
            shopOrders.GradeList
                        .Select(ValidateStudentGrade(checkOrderExists))
                        .Aggregate(CreateEmptyValatedGradesList().ToAsync(), ReduceValidGrades)
                        .MatchAsync(
                            Right: validatedOrders => new ValidatedShopOrders(validatedOrders),
                            LeftAsync: errorMessage => Task.FromResult((IShopOrder) new InvalidatedShopOrders(shopOrders.GradeList, errorMessage))
                        );

        private static Func<UnvalidatedOrder, EitherAsync<string, ValidatedOrder>> ValidateStudentGrade(Func<OrderRegistrationNumber, Option<OrderRegistrationNumber>> checkStudentExists) =>
            unvalidatedStudentGrade => ValidateStudentGrade(checkStudentExists, unvalidatedStudentGrade);

        private static EitherAsync<string, ValidatedOrder> ValidateStudentGrade(Func<OrderRegistrationNumber, Option<OrderRegistrationNumber>> checkStudentExists, UnvalidatedOrder unvalidatedGrade) =>
            from examGrade in Order.TryParseOrder(unvalidatedGrade.Grade)
                                   .ToEitherAsync($"Invalid exam grade ({unvalidatedGrade.OrderRegistrationNumber}, {unvalidatedGrade.Grade})")
            from studentRegistrationNumber in OrderRegistrationNumber.TryParse(unvalidatedGrade.OrderRegistrationNumber)
                                   .ToEitherAsync($"Invalid student registration number ({unvalidatedGrade.OrderRegistrationNumber})")
            from studentExists in checkStudentExists(studentRegistrationNumber)
                                   .ToEitherAsync($"Student {studentRegistrationNumber.Value} does not exist.")
            select new ValidatedOrder(studentRegistrationNumber, examGrade) { OrderId = examGrade.OrderId};

        private static Either<string, List<ValidatedOrder>> CreateEmptyValatedGradesList() =>
            Right(new List<ValidatedOrder>());


        private static EitherAsync<string, List<ValidatedOrder>> ReduceValidGrades(EitherAsync<string, List<ValidatedOrder>> acc, EitherAsync<string, ValidatedOrder> next) =>
            from list in acc
            from nextGrade in next
            select list.AppendValidGrade(nextGrade);

        private static List<ValidatedOrder> AppendValidGrade(this List<ValidatedOrder> list, ValidatedOrder validGrade)
        {
            list.Add(validGrade);
            return list;
        }

        public static IShopOrder CalculateFinalGrades(IShopOrder examGrades) => examGrades.Match(
            whenUnvalidatedShopOrders: unvalidaTedExam => unvalidaTedExam,
            whenInvalidatedShopOrders: invalidExam => invalidExam,
            whenCalculatedShopOrders: calculatedExam => calculatedExam,
            whenCancelledShopOrder: publishedExam => publishedExam,
            whenFailedShopOrders: failedExam => failedExam,
            whenValidatedShopOrders: CalculateFinalGrade
        );

        private static IShopOrder CalculateFinalGrade(ValidatedShopOrders validExamGrades) =>
            new CalculatedShopOrders(validExamGrades.GradeList
                                                    .Select(CalculateStudentFinalGrade)
                                                    .ToList()
                                                    .AsReadOnly());

        private static CalculatedOrder CalculateStudentFinalGrade(ValidatedOrder validGrade) =>
            new CalculatedOrder(validGrade.OrderRegistrationNumber,
                                      validGrade.Order,
                                      validGrade.Order.AddVAT());


        public static IShopOrder PublishExamGrades(IShopOrder examGrades) => examGrades.Match(
            whenUnvalidatedShopOrders: unvalidaTedExam => unvalidaTedExam,
            whenInvalidatedShopOrders: invalidExam => invalidExam,
            whenFailedShopOrders: failedExam => failedExam,
            whenValidatedShopOrders: validatedExam => validatedExam,
            whenCancelledShopOrder: publishedExam => publishedExam,
            whenCalculatedShopOrders: GenerateExport);

        private static IShopOrder GenerateExport(CalculatedShopOrders calculatedExam) =>
            new CancelledShopOrder(calculatedExam.GradeList,
                                    calculatedExam.GradeList.Aggregate(new StringBuilder(), CreateCsvLine).ToString(),
                                    DateTime.Now);

        private static StringBuilder CreateCsvLine(StringBuilder export, CalculatedOrder grade) =>
            export.AppendLine($"{grade.StudentRegistrationNumber.Value}, {grade.Order}, {grade.FinalOrder}");
    }
}
