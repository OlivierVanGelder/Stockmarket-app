import React from "react";
import { useState, useEffect } from "react";
import "../App.css";
import LineChart from "../components/LineChart";
import CandleStickChart from "../components/CandleStickChart";

async function fetchStockData(
  ticker: string,
  interval: number = 1,
  startDay: number,
  endDay: number
) {
  try {
    const response = await fetch(
      `https://localhost:42069/stock?ticker=${ticker}&interval=${interval}&start=${startDay}&end=${endDay}`
    );
    if (!response.ok) {
      throw new Error("Failed to fetch stock data");
    }
    return await response.json();
  } catch (error) {
    console.error(`Error fetching data for ${ticker}:`, error);
    return null;
  }
}

function extractData(
  stockData: number[],
  interval: number = 1, // Interval in days
  startDay: number
) {
  const labels: string[] = [];

  // Base date is "1980-01-01T12:00:00"
  const baseDate = new Date("1980-01-01T12:00:00");

  for (let i = 0; i < stockData.length; i++) {
    // Calculate the total increment in days, hours, or minutes
    const totalIncrement = i * interval + startDay;

    // Create a new date based on the total increment
    const currentDate = new Date(baseDate);

    // Increment by days
    currentDate.setDate(baseDate.getDate() + Math.floor(totalIncrement));

    // Calculate the fractional part for hours and minutes
    const fractionalPart = totalIncrement - Math.floor(totalIncrement);

    // Increment by hours (1 hour = 0.04166666667 days)
    currentDate.setHours(
      baseDate.getHours() + Math.floor(fractionalPart / 0.04166666667)
    );

    // Increment by minutes (1 minute = 0.0006944444444 days)
    const remainingFraction = fractionalPart % 0.04166666667;
    currentDate.setMinutes(
      baseDate.getMinutes() + Math.floor(remainingFraction / 0.0006944444444)
    );

    // Format the date as "YYYY-MM-DDTHH:mm"
    const formattedDate = currentDate.toISOString().slice(0, 16); // Use slice(0, 16) to omit seconds
    labels.push(formattedDate);
  }

  return {
    labels: labels,
    data: stockData,
  };
}

function Graphs() {
  const [userDataIBM, setUserDataIBM] = useState<{
    labels: string[];
    datasets: any[];
  }>({
    labels: [],
    datasets: [],
  });
  const [userDataAMZN, setUserDataAMZN] = useState<{
    labels: string[];
    datasets: any[];
  }>({
    labels: [],
    datasets: [],
  });
  const [userDataTSLA, setUserDataTSLA] = useState<{
    labels: string[];
    datasets: any[];
  }>({
    labels: [],
    datasets: [],
  });
  const [userDataAPPL, setUserDataAPPL] = useState<{
    labels: string[];
    datasets: any[];
  }>({
    labels: [],
    datasets: [],
  });
  const [userDataGOOG, setUserDataGOOG] = useState<{
    labels: string[];
    datasets: any[];
  }>({
    labels: [],
    datasets: [],
  });
  const [userDataMSFT, setUserDataMSFT] = useState<{
    labels: string[];
    datasets: any[];
  }>({
    labels: [],
    datasets: [],
  });

  const userDatas = [
    userDataIBM,
    userDataAMZN,
    userDataTSLA,
    userDataAPPL,
    userDataGOOG,
    userDataMSFT,
  ];

  async function setUserData(
    ticker: string,
    interval: number,
    startDay: number,
    endDay: number,
    setUserData: any,
    color: string[]
  ) {
    const newStockData = await fetchStockData(
      ticker,
      interval,
      startDay,
      endDay
    );
    if (newStockData) {
      const extractedData = extractData(newStockData, interval, startDay);
      setUserData({
        labels: extractedData.labels,
        datasets: [
          {
            label: `Stock Price ${ticker}`,
            data: extractedData.data,
            backgroundColor: color,
            borderColor: color,
          },
        ],
      });
    }
  }

  useEffect(() => {
    const fetchData = async () => {
      await setUserData("IBM", 0.017, 15000, 15001, setUserDataIBM, [
        "rgb(242, 139, 130)",
      ]);
      await setUserData("AMZN", 0.017, 15000, 15001, setUserDataAMZN, [
        "rgb(129, 201, 149)",
      ]);
      await setUserData("TSLA", 0.017, 15000, 15001, setUserDataTSLA, [
        "rgb(2, 163, 212)",
      ]);
      await setUserData("APPL", 0.017, 15000, 15001, setUserDataAPPL, [
        "rgb(245, 185, 66)",
      ]);
      await setUserData("GOGL", 0.017, 15000, 15001, setUserDataGOOG, [
        "rgb(183, 40, 235)",
      ]);
      await setUserData("MSFT", 0.017, 15000, 15001, setUserDataMSFT, [
        "rgb(150, 237, 9)",
      ]);
    };

    fetchData();
  }, []);

  return (
    <div>
      <CandleStickChart />
      {userDatas.map((element, index) => (
        <div
          key={index}
          style={{ width: 1200, backgroundColor: "#444746", margin: "35px" }}
        >
          <LineChart chartData={element} />
        </div>
      ))}
    </div>
  );
}

export default Graphs;
