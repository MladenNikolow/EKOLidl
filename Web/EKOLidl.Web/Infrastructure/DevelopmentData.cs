namespace EKOLidl.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using EKOLidl.Data.Models;
    using EKOLidl.Data.Models.Events;
    using EKOLidl.Data.Models.Signals;
    using EKOLidl.Services.Data.CosmosDb;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Prepares local emulators (Cosmos DB emulator and Azurite) for development: creates the
    ///     database, containers and blob container, and seeds sample content when the containers are empty.
    ///     Runs only in the Development environment.
    /// </summary>
    public static class DevelopmentData
    {
        private const string SampleImages = "https://ekolidlstorage.blob.core.windows.net/ekolidlblobs/";

        public static async Task InitializeAsync(IServiceProvider services, CosmosDbSettings cosmosDbSettings)
        {
            ILogger logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DevelopmentData));

            try
            {
                await CosmosDbInitializer.EnsureCreatedAsync(services.GetRequiredService<CosmosClient>(), cosmosDbSettings);
                await services.GetRequiredService<BlobContainerClient>().CreateIfNotExistsAsync(PublicAccessType.Blob);

                using IServiceScope scope = services.CreateScope();
                IEventRepository events = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                ISignalRepository signals = scope.ServiceProvider.GetRequiredService<ISignalRepository>();

                if (await events.GetItemsCountAsync() == 0)
                {
                    foreach (Event item in SampleEvents())
                    {
                        await events.AddItemAsync(item);
                    }

                    logger.LogInformation("Seeded sample events.");
                }

                if (await signals.GetItemsCountAsync() == 0)
                {
                    foreach (Signal item in SampleSignals())
                    {
                        await signals.AddItemAsync(item);
                    }

                    logger.LogInformation("Seeded sample signals.");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Local data stores are not reachable. Start the Cosmos DB emulator and Azurite (see README).");
            }
        }

        private static Album Photos(params string[] files)
        {
            List<string> links = new List<string>();
            foreach (string file in files)
            {
                links.Add(SampleImages + file);
            }

            return new Album { Links = links };
        }

        private static IEnumerable<Event> SampleEvents()
        {
            yield return new Event
            {
                Title = "Неформални обучения",
                ShortDescription = "17 обучения на деца и младежи от Варна, Каварна и Шабла на тема „Накрая стигам до морето“.",
                LongDescription = "От март до юни провеждаме неформални обучения за ученици на възраст от 12 до 18 години. Говорим за пътя на отпадъците до морето и за това как всеки може да го прекъсне.",
                EventDate = new DateTime(2022, 3, 15, 10, 0, 0, DateTimeKind.Utc),
                Duration = 2,
                Album = Photos("front_background.jpg", "background.jpg"),
                Result = new Result { Description = "Обученията събраха деца и младежи от три общини.", Album = Photos("background.jpg") },
            };

            yield return new Event
            {
                Title = "На каквото си постелеш, на това ще легнеш",
                ShortDescription = "8 кампании за почистване на неохраняеми плажове от Варна до Шабла.",
                LongDescription = "От юли до октомври организираме осем кампании за почистване на неохраняеми плажове по северното черноморско крайбрежие.",
                EventDate = new DateTime(2022, 7, 9, 9, 0, 0, DateTimeKind.Utc),
                Duration = 4,
                Album = Photos("background.jpg", "front_background.jpg"),
                Result = new Result { Description = "Още няма добавени резултати!", Album = new Album { Links = new List<string>() } },
            };

            yield return new Event
            {
                Title = "Фото-лов",
                ShortDescription = "Снимаме замърсени крайбрежни райони и нерегламентирани сметища.",
                LongDescription = "Участниците документират замърсени места по северното крайбрежие и ги подават като сигнали в платформата.",
                EventDate = new DateTime(2022, 8, 20, 9, 0, 0, DateTimeKind.Utc),
                Duration = 3,
                Album = Photos("justin-hofman.m.jpg"),
                Result = new Result { Description = "Още няма добавени резултати!", Album = new Album { Links = new List<string>() } },
            };

            yield return new Event
            {
                Title = "Международен ден на Черно море",
                ShortDescription = "Заключително събитие по случай 31 октомври.",
                LongDescription = "Отбелязваме Международния ден на Черно море със заключително събитие на проекта.",
                EventDate = new DateTime(2022, 10, 31, 11, 0, 0, DateTimeKind.Utc),
                Duration = 3,
                Album = Photos("front_background.jpg"),
                Result = new Result { Description = "Още няма добавени резултати!", Album = new Album { Links = new List<string>() } },
            };
        }

        private static IEnumerable<Signal> SampleSignals()
        {
            Reporter reporter = new Reporter { Names = "Примерен Подател", Gsm = "0888000000", Email = "sample@example.com" };

            yield return new Signal
            {
                Title = "Плаващи отпадъци край Варна",
                Description = "Пластмасови бутилки и найлонови торби плуват близо до брега.",
                TrashType = new List<TrashType> { TrashType.Plastic },
                Location = "Варна, Южен плаж",
                Album = Photos("background.jpg"),
                Reporter = reporter,
                IsApproved = true,
                Result = new Result { Description = "Още няма добавени резултати!", Album = new Album { Links = new List<string>() } },
            };

            yield return new Signal
            {
                Title = "Отпадъци на плажа в Каварна",
                Description = "Стъкло и метални кутии, изхвърлени между скалите след уикенда.",
                TrashType = new List<TrashType> { TrashType.Glass, TrashType.Metal },
                Location = "Каварна, плаж Каварна",
                Album = Photos("front_background.jpg"),
                Reporter = reporter,
                IsApproved = true,
                Result = new Result { Description = "Мястото беше почистено от доброволци.", Album = Photos("background.jpg") },
            };

            yield return new Signal
            {
                Title = "Нерегламентирано сметище",
                Description = "Купчина строителни и битови отпадъци в дерето над плажа.",
                TrashType = new List<TrashType> { TrashType.Other },
                OtherTrashType = "Строителни отпадъци",
                Location = "Шабла, до фара",
                Album = Photos("justin-hofman.m.jpg"),
                Reporter = reporter,
                IsApproved = true,
                Result = new Result { Description = "Още няма добавени резултати!", Album = new Album { Links = new List<string>() } },
            };

            yield return new Signal
            {
                Title = "Сигнал, който чака одобрение",
                Description = "Този сигнал не е одобрен и не трябва да се вижда публично.",
                TrashType = new List<TrashType> { TrashType.Paper },
                Location = "Варна, Морска градина",
                Album = Photos("background.jpg"),
                Reporter = reporter,
                IsApproved = false,
                Result = new Result { Description = "Още няма добавени резултати!", Album = new Album { Links = new List<string>() } },
            };
        }
    }
}
