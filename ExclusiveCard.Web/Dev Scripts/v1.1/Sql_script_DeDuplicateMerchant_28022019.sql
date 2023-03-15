-- create duplicate merchant list with master merchant id
Create Table #TEMP(
RowId int identity(1,1),
masterMerchantId int, 
dupMerchantId int
);

--seeding values to temp table
insert into #TEMP values(1365,1406);
insert into #TEMP values(815,1775);
insert into #TEMP values(690,3005);
insert into #TEMP values(1405,1299);
insert into #TEMP values(1284,3092);
insert into #TEMP values(1106,2896);
insert into #TEMP values(1138,3217);
insert into #TEMP values(1138,2534);
insert into #TEMP values(1138,1782);
insert into #TEMP values(1138,2078);
insert into #TEMP values(1240,1239);
insert into #TEMP values(1118,2537);
insert into #TEMP values(1118,2039);
insert into #TEMP values(1118,3348);
insert into #TEMP values(1118,3019);
insert into #TEMP values(1058,1448);
insert into #TEMP values(1319,3334);
insert into #TEMP values(700,3233);
insert into #TEMP values(636,1385);
insert into #TEMP values(1308,3019);
insert into #TEMP values(435,2418);
insert into #TEMP values(444,1905);
insert into #TEMP values(1228,2399);
insert into #TEMP values(874,3282);
insert into #TEMP values(3218,2545);
insert into #TEMP values(817,2547);
insert into #TEMP values(2169,2548);
insert into #TEMP values(918,3052);
insert into #TEMP values(918,2549);
insert into #TEMP values(614,2081);
insert into #TEMP values(945,2987);
insert into #TEMP values(662,1911);
insert into #TEMP values(1070,2281);
insert into #TEMP values(871,2842);
insert into #TEMP values(950,1396);
insert into #TEMP values(850,2351);
insert into #TEMP values(1120,3286);
insert into #TEMP values(1355,3531);
insert into #TEMP values(876,3302);
insert into #TEMP values(1143,3161);
insert into #TEMP values(295,2019);
insert into #TEMP values(946,3264);
insert into #TEMP values(266,2436);
insert into #TEMP values(266,1377);
insert into #TEMP values(555,1395);
insert into #TEMP values(195,1409);
insert into #TEMP values(539,1375);
insert into #TEMP values(471,1428);
insert into #TEMP values(390,1455);
insert into #TEMP values(1170,1419);
insert into #TEMP values(859,1988);
insert into #TEMP values(1233,1577);
insert into #TEMP values(567,1441);
insert into #TEMP values(697,3075);
insert into #TEMP values(2569,3219);
insert into #TEMP values(3202,2954);
insert into #TEMP values(1035,2940);
insert into #TEMP values(747,2126);
insert into #TEMP values(680,2463);
insert into #TEMP values(442,1862);
insert into #TEMP values(738,2103);
insert into #TEMP values(1180,3296);
insert into #TEMP values(518,2263);
insert into #TEMP values(1132,3251);
insert into #TEMP values(1132,2577);
insert into #TEMP values(914,1809);
insert into #TEMP values(916,2259);
insert into #TEMP values(3223,2579);
insert into #TEMP values(1149,2932);
insert into #TEMP values(940,2043);
insert into #TEMP values(1325,2999);
insert into #TEMP values(477,1389);
insert into #TEMP values(1067,2411);
insert into #TEMP values(791,1423);
insert into #TEMP values(954,2289);
insert into #TEMP values(954,2587);
insert into #TEMP values(1241,3227);
insert into #TEMP values(3248,2209);
insert into #TEMP values(932,2482);
insert into #TEMP values(834,1790);
insert into #TEMP values(930,2226);
insert into #TEMP values(522,3155);
insert into #TEMP values(522,2593);
insert into #TEMP values(3462,3483);
insert into #TEMP values(724,3071);
insert into #TEMP values(1324,3259);
insert into #TEMP values(296,2028);
insert into #TEMP values(558,2042);
insert into #TEMP values(848,2274);
insert into #TEMP values(1134,1794);
insert into #TEMP values(2073,2075);
insert into #TEMP values(1310,2228);
insert into #TEMP values(2434,2465);
insert into #TEMP values(2434,2596);
insert into #TEMP values(646,1952);
insert into #TEMP values(735,3056);
insert into #TEMP values(735,2889);
insert into #TEMP values(1161,2597);
insert into #TEMP values(1230,2421);
insert into #TEMP values(1230,1384);
insert into #TEMP values(2506,1977);
insert into #TEMP values(1071,1407);
insert into #TEMP values(865,2122);
insert into #TEMP values(606,629);
insert into #TEMP values(606,1499);
insert into #TEMP values(1119,2945);
insert into #TEMP values(1044,2601);
insert into #TEMP values(1895,2602);
insert into #TEMP values(1313,3482);
insert into #TEMP values(882,2879);
insert into #TEMP values(1532,1951);
insert into #TEMP values(862,2023);
insert into #TEMP values(436,1740);
insert into #TEMP values(763,2166);
insert into #TEMP values(702,3254);
insert into #TEMP values(286,2871);
insert into #TEMP values(305,1373);
insert into #TEMP values(1311,1656);
insert into #TEMP values(1296,2522);
insert into #TEMP values(919,3333);
insert into #TEMP values(523,3156);
insert into #TEMP values(523,2609);
insert into #TEMP values(873,1393);
insert into #TEMP values(873,3070);
insert into #TEMP values(620,1595);
insert into #TEMP values(613,1731);
insert into #TEMP values(1062,2361);
insert into #TEMP values(1539,2139);
insert into #TEMP values(422,2615);
insert into #TEMP values(1254,3287);
insert into #TEMP values(806,1786);
insert into #TEMP values(308,2467);
insert into #TEMP values(308,2466);
insert into #TEMP values(308,1383);
insert into #TEMP values(308,2468);
insert into #TEMP values(2618,1840);
insert into #TEMP values(677,1559);
insert into #TEMP values(911,1391);
insert into #TEMP values(1045,3103);
insert into #TEMP values(1091,3138);
insert into #TEMP values(685,2501);
insert into #TEMP values(306,2512);
insert into #TEMP values(306,1714);
insert into #TEMP values(683,1823);
insert into #TEMP values(683,1813);
insert into #TEMP values(511,1671);
insert into #TEMP values(2640,3222);
insert into #TEMP values(1300,2240);
insert into #TEMP values(800,1965);
insert into #TEMP values(120,1374);
insert into #TEMP values(603,2095);
insert into #TEMP values(302,3490);
insert into #TEMP values(906,3280);
insert into #TEMP values(906,3154);
insert into #TEMP values(748,2203);
insert into #TEMP values(482,1889);
insert into #TEMP values(836,1872);
insert into #TEMP values(2929,3278);
insert into #TEMP values(1304,2208);
insert into #TEMP values(1030,2826);
insert into #TEMP values(892,2247);
insert into #TEMP values(3198,2645);
insert into #TEMP values(1104,3020);
insert into #TEMP values(707,3244);
insert into #TEMP values(274,1538);
insert into #TEMP values(386,3453);
insert into #TEMP values(1232,1935);
insert into #TEMP values(1188,3327);
insert into #TEMP values(598,1781);
insert into #TEMP values(2928,1586);
insert into #TEMP values(818,1470);
insert into #TEMP values(1145,3094);
insert into #TEMP values(708,3238);
insert into #TEMP values(1213,2246);
insert into #TEMP values(510,1531);
insert into #TEMP values(575,2358);
insert into #TEMP values(575,2264);
insert into #TEMP values(575,2355);
insert into #TEMP values(575,1390);
insert into #TEMP values(1247,3109);
insert into #TEMP values(282,1523);
insert into #TEMP values(894,1506);
insert into #TEMP values(1224,1522);
insert into #TEMP values(1181,2431);
insert into #TEMP values(667,2456);
insert into #TEMP values(2190,3375);
insert into #TEMP values(1055,3201);
insert into #TEMP values(1055,2674);
insert into #TEMP values(537,2088);
insert into #TEMP values(441,1547);
insert into #TEMP values(300,1382);
insert into #TEMP values(591,2113);
insert into #TEMP values(1222,3128);
insert into #TEMP values(2989,2676);
insert into #TEMP values(753,1371);
insert into #TEMP values(759,1710);
insert into #TEMP values(1235,2895);
insert into #TEMP values(1215,2845);
insert into #TEMP values(1215,2496);
insert into #TEMP values(1215,1653);
insert into #TEMP values(1204,3356);
insert into #TEMP values(807,1792);
insert into #TEMP values(254,1521);
insert into #TEMP values(725,3091);
insert into #TEMP values(725,2684);
insert into #TEMP values(686,1476);
insert into #TEMP values(675,2525);
insert into #TEMP values(1043,3249);
insert into #TEMP values(809,1451);
insert into #TEMP values(1103,3007);
insert into #TEMP values(1137,3014);
insert into #TEMP values(426,3134);
insert into #TEMP values(2473,2310);
insert into #TEMP values(2473,2171);
insert into #TEMP values(851,2693);
insert into #TEMP values(851,2345);
insert into #TEMP values(2047,2922);
insert into #TEMP values(869,2815);
insert into #TEMP values(823,1418);
insert into #TEMP values(711,3246);
insert into #TEMP values(113,1386);
insert into #TEMP values(1172,1930);
insert into #TEMP values(521,1387);
insert into #TEMP values(124,1701);
insert into #TEMP values(726,3310);
insert into #TEMP values(726,3121);
insert into #TEMP values(1260,3212);
insert into #TEMP values(560,1801);
insert into #TEMP values(507,1677);
insert into #TEMP values(713,3197);
insert into #TEMP values(713,2700);
insert into #TEMP values(1642,1643);
insert into #TEMP values(2027,3461);
insert into #TEMP values(958,1413);
insert into #TEMP values(3090,3521);
insert into #TEMP values(715,2930);
insert into #TEMP values(1029,1932);
insert into #TEMP values(375,1891);
insert into #TEMP values(443,1415);
insert into #TEMP values(2918,1505);
insert into #TEMP values(446,1520);
insert into #TEMP values(1321,2051);
insert into #TEMP values(774,2707);
insert into #TEMP values(2449,641);
insert into #TEMP values(1025,3312);
insert into #TEMP values(663,2110);
insert into #TEMP values(2710,2365);
insert into #TEMP values(1950,2279);
insert into #TEMP values(478,1370);
insert into #TEMP values(722,3151);
insert into #TEMP values(722,2714);
insert into #TEMP values(3127,2715);
insert into #TEMP values(727,2924);
insert into #TEMP values(1122,3255);
insert into #TEMP values(920,3106);
insert into #TEMP values(1366,1401);
insert into #TEMP values(2778,3115);
insert into #TEMP values(814,1463);
insert into #TEMP values(1124,3038);
insert into #TEMP values(855,1745);
insert into #TEMP values(1185,3034);
insert into #TEMP values(1182,3241);
insert into #TEMP values(730,2899);
insert into #TEMP values(623,1583);
insert into #TEMP values(264,1369);
insert into #TEMP values(1109,2420);
insert into #TEMP values(687,1785);
insert into #TEMP values(1113,3211);
insert into #TEMP values(1113,1460);
insert into #TEMP values(289,1799);
insert into #TEMP values(936,1571);
insert into #TEMP values(884,1995);
insert into #TEMP values(277,1974);
insert into #TEMP values(1116,3316);
insert into #TEMP values(383,1566);
insert into #TEMP values(440,2272);
insert into #TEMP values(574,2491);
insert into #TEMP values(574,1573);
insert into #TEMP values(294,1726);
insert into #TEMP values(573,1829);
insert into #TEMP values(2785,2786);
insert into #TEMP values(2233,3139);
insert into #TEMP values(1245,3339);
insert into #TEMP values(1148,3111);
insert into #TEMP values(650,1404);
insert into #TEMP values(655,2441);
insert into #TEMP values(789,1422);
insert into #TEMP values(929,2741);
insert into #TEMP values(901,1429);
insert into #TEMP values(1083,1602);
insert into #TEMP values(293,1558);
insert into #TEMP values(669,1964);
insert into #TEMP values(1038,2128);
insert into #TEMP values(822,1540);
insert into #TEMP values(1050,1491);
insert into #TEMP values(931,2470);
insert into #TEMP values(519,2427);
insert into #TEMP values(2261,2748);
insert into #TEMP values(841,1861);
insert into #TEMP values(458,2020);
insert into #TEMP values(458,1647);
insert into #TEMP values(2967,2347);
insert into #TEMP values(3423,1443);
insert into #TEMP values(203,204);
insert into #TEMP values(935,2394);
insert into #TEMP values(3285,2148);
insert into #TEMP values(895,1376);
insert into #TEMP values(899,1513);
insert into #TEMP values(1317,3498);
insert into #TEMP values(1154,2335);
insert into #TEMP values(734,3174);
insert into #TEMP values(826,2846);
insert into #TEMP values(3081,2765);
insert into #TEMP values(1179,1907);
insert into #TEMP values(1054,3200);
insert into #TEMP values(1054,2767);
insert into #TEMP values(3126,2768);
insert into #TEMP values(757,2498);
insert into #TEMP values(757,1380);
insert into #TEMP values(1246,2921);
insert into #TEMP values(645,1620);
insert into #TEMP values(1026,2432);

DECLARE @count INT = 0;
DECLARE @index INT = 1;
SELECT @count = COUNT(*)
 FROM #TEMP

-- while do until count = table last value
WHILE(@index <= @count)
BEGIN

	BEGIN TRY	
		BEGIN TRAN

		DECLARE @masterId int;
		DECLARE @dupId int;
		DECLARE @dupContactId int = NULL;
		DECLARE @masterContactId int = NULL;

		SELECT @masterId = x.masterMerchantId, @dupId = x.dupMerchantId
		FROM #TEMP x where x.RowID = @index

	-- 1. offers move to merchant id
		--select * from Exclusive.Offer
		update Exclusive.Offer set MerchantId = @masterId
		where MerchantId = @dupId

	-- 2. AWIN Mappings table update with master merchant id
		--select * from Exclusive.AffiliateMapping
		--select * from Exclusive.AffiliateMappingRule
		update Exclusive.AffiliateMapping set ExclusiveValue = CAST(@masterId as nvarchar(max))
		where ExclusiveValue = CAST(@dupId as nvarchar(max)) 
		and AffiliateMappingRuleId = (Select id from Exclusive.AffiliateMappingRule where Description = 'AWIN Merchants' and IsActive = 1)

	-- 3. branches move to master merchant id
		--select * from Exclusive.MerchantBranch
		update Exclusive.MerchantBranch set MerchantId = @masterId
		where MerchantId = @dupId and IsDeleted = 0
	-- 4. contactdetails move to master merchant id
		select @dupContactId = ContactDetailsId from Exclusive.Merchant where id = @dupId
		IF(@dupContactId != NULL)
		BEGIN
			Select @masterContactId = ContactDetailsId from Exclusive.Merchant where id = @masterId
			update Exclusive.Merchant set ContactDetailsId = @dupContactId where Id = @masterId
			IF(@masterContactId != NULL)
			BEGIN
			-- 5. soft delete of existing master contactdetails
				update exclusive.ContactDetail set IsDeleted = 1 where Id = @masterContactId and IsDeleted = 0
			END
		END
	-- 6. soft delete of dupmerchant id
		update Exclusive.Merchant set IsDeleted = 1 where Id = @dupId and IsDeleted = 0

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		PRINT 'Error in process of masterMerchantId: '+ @masterId + ' and dupMerchantId: ' + @dupId
		IF(@@TRANCOUNT > 0)
			ROLLBACK TRAN;
	END CATCH
	SET @index = @index + 1;
END

DROP TABLE #TEMP