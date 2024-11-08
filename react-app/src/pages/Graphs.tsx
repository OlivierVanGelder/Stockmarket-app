import React, { useState, useEffect, useRef } from "react";
import "../App.css";
import LineChart from "../components/LineChart";
import CandleStickChart from "../components/CandleStickChart";
import CandleItem from "../CandleItem";
import ToggleButtonNotEmpty from "../components/ToggleButton";
import { start } from "repl";
import { Console } from "console";

async function fetchStockNames(): Promise<string[]> {
  try {
    const response = await fetch("https://localhost:42069/stocknames");
    if (!response.ok) {
      throw new Error("Failed to fetch stock names");
    }
    return await response.json();
  } catch (error) {
    console.error("Error fetching stock names:", error);
    return [];
  }
}

function convertToDays(date: Date): number {
  const referenceDate = new Date("2024-11-01T12:00:00Z");

  const diffInMs = date.getTime() - referenceDate.getTime();

  // Convert the difference from milliseconds to days
  const diffInDays = diffInMs / (1000 * 60 * 60 * 24); // 1000 ms * 60 s * 60 min * 24 hrs

  return parseFloat(diffInDays.toFixed(10));
}

function extractData(
  stockAmount: number,
  interval: number = 1,
  startDay: number
) {
  const labels: string[] = [];
  const baseDate = new Date("2024-11-01T12:00:00");

  for (let i = 0; i < stockAmount; i++) {
    const totalIncrement = i * interval + startDay;
    const currentDate = new Date(baseDate);
    currentDate.setDate(baseDate.getDate() + Math.floor(totalIncrement));
    const fractionalPart = totalIncrement - Math.floor(totalIncrement);
    currentDate.setHours(
      baseDate.getHours() + Math.floor(fractionalPart / 0.04166666667)
    );
    const remainingFraction = fractionalPart % 0.04166666667;
    currentDate.setMinutes(
      baseDate.getMinutes() + Math.floor(remainingFraction / 0.0006944444444)
    );
    const formattedDate = currentDate.toISOString().slice(0, 16);
    labels.push(formattedDate);
  }

  return {
    labels: labels,
  };
}

function Graphs() {
  const [candleSelected, setCandleSelected] = useState<boolean>(false);
  const [ticker, setTicker] = useState<string>("AAPL");
  const [candleData, setCandleData] = useState<CandleDataItem[]>([]);
  const [interval, setInterval] = useState<number>(1 / 24);
  const [startDay, setStartDay] = useState<string>("all");
  const tickerRef = useRef(ticker);
  const intervalRef = useRef(interval);
  const startDayRef = useRef(startDay);
  const [chartData, setUserData] = useState<{
    labels: string[];
    datasets: any[];
  }>({
    labels: [],
    datasets: [],
  });
  const [stocknames, setStockNames] = useState<string[]>([]);
  const [socket, setSocket] = useState<WebSocket | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      setStockNames(await fetchStockNames());
    };
    fetchData();
  }, []);

  useEffect(() => {
    tickerRef.current = ticker;
  }, [ticker]);

  useEffect(() => {
    intervalRef.current = interval;
  }, [interval]);

  useEffect(() => {
    startDayRef.current = startDay;
  }, [startDay]);

  useEffect(() => {
    const newSocket = new WebSocket(`wss://localhost:42069/stockWS`);

    newSocket.onopen = () => {
      console.log("Connected to websocket");
      const currentDate = new Date();
      var startTime: number = 0;
      switch (startDay) {
        case "hour":
          currentDate.setHours(currentDate.getHours() - 1);
          break;
        case "hours":
          currentDate.setHours(currentDate.getHours() - 6);
          break;
        case "day":
          currentDate.setDate(currentDate.getDay() - 1);
          break;
        case "week":
          currentDate.setDate(currentDate.getDay() - 7);
          break;
        case "month":
          currentDate.setMonth(currentDate.getMonth() - 1);
          break;
        case "year":
          currentDate.setFullYear(currentDate.getFullYear() - 1);
          break;
        case "all":
          currentDate.setFullYear(currentDate.getFullYear() - 100);
          break;
      }
      startTime = convertToDays(currentDate);
      if (startTime < 0) {
        startTime = 0;
      }
      const endDay = convertToDays(new Date());
      if (candleSelected) {
        newSocket.send(
          JSON.stringify(`${ticker}-${interval}-${startTime}-${endDay}-candle`)
        );
        console.log(`${ticker}-${interval}-${startTime}-${endDay}-candle`);
      } else if (!candleSelected) {
        newSocket.send(
          JSON.stringify(`${ticker}-${interval}-${startTime}-${endDay}`)
        );
      }
    };

    newSocket.onmessage = (event) => {
      const newData = JSON.parse(event.data);
      console.log("Received data:", newData);
      const tickerValue = tickerRef.current;
      const intervalValue = intervalRef.current;
      const startDayValue = startDayRef.current;
      const currentDate = new Date();
      var startTime: number = 0;
      switch (startDay) {
        case "hour":
          currentDate.setHours(currentDate.getHours() - 1);
          break;
        case "hours":
          currentDate.setHours(currentDate.getHours() - 6);
          break;
        case "day":
          currentDate.setDate(currentDate.getDay() - 1);
          break;
        case "week":
          currentDate.setDate(currentDate.getDay() - 7);
          break;
        case "month":
          currentDate.setMonth(currentDate.getMonth() - 1);
          break;
        case "year":
          currentDate.setFullYear(currentDate.getFullYear() - 1);
          break;
        case "all":
          currentDate.setFullYear(currentDate.getFullYear() - 100);
          break;
      }
      startTime = convertToDays(currentDate);
      if (startTime < 0) {
        startTime = 0;
      }
      const endDay = convertToDays(new Date());
      if (candleSelected) {
        ApplyCandleData(
          tickerValue,
          interval,
          startTime,
          setCandleData,
          newData
        );
      } else {
        applyLineData(
          tickerValue,
          intervalValue,
          startTime,
          endDay,
          setUserData,
          ["rgb(242, 139, 130)"],
          newData
        );
      }
    };

    setSocket(newSocket);

    return () => {
      newSocket.close();
    };
  }, [interval, startDay, candleSelected, ticker]);

  async function applyLineData(
    ticker: string,
    interval: number,
    startDay: number,
    endDay: number,
    setUserData: React.Dispatch<
      React.SetStateAction<{ labels: string[]; datasets: any[] }>
    >,
    color: string[],
    newStockData: any
  ) {
    if (newStockData) {
      const extractedData = extractData(
        newStockData.length,
        interval,
        startDay
      );
      setUserData({
        labels: extractedData.labels,
        datasets: [
          {
            label: `Stock Price for ${ticker}`,
            data: newStockData,
            backgroundColor: color,
            borderColor: color,
          },
        ],
      });
    }
  }

  interface CandleDataItem {
    x: Date;
    y: [open: number, high: number, low: number, close: number];
    volume: number;
  }

  async function ApplyCandleData(
    ticker: string,
    interval: number,
    startDay: number,
    setCandleData: React.Dispatch<React.SetStateAction<CandleDataItem[]>>,
    newCandleData: any
  ) {
    // Check if newCandleData is an array and has valid data
    if (!Array.isArray(newCandleData) || newCandleData.length === 0) {
      console.error("Invalid candle data received:", newCandleData);
      return; // Exit if data is invalid
    }

    const extractedData = extractData(newCandleData.length, interval, startDay);

    const candleData = newCandleData.reduce<CandleDataItem[]>(
      (acc, item, index) => {
        if (item && typeof item === "object") {
          acc.push({
            x: new Date(extractedData.labels[index]), // Use the corresponding date
            y: [item.Open, item.High, item.Low, item.Close],
            volume: item.Volume,
          });
        } else {
          console.error("Invalid candle item format:", item);
        }
        return acc;
      },
      []
    ); // Start with an empty array

    // Set the valid candle data
    setCandleData(candleData);
  }

  return (
    <div>
      <select
        value={ticker}
        onChange={(e) => {
          const value: string = e.target.value.toString();
          setTicker(value);
        }}
      >
        {stocknames.map((element, index) => (
          <option key={index} value={element}>
            {element}
          </option>
        ))}
      </select>

      <select
        value={startDayRef.current}
        onChange={(e) => {
          const value: string = e.target.value;
          setStartDay(value);
        }}
      >
        <option value={"hour"}>{"1 hour"}</option>
        <option value={"hours"}>{"6 hour"}</option>
        <option value={"day"}>{"1 day"}</option>
        <option value={"week"}>{"1 week"}</option>
        <option value={"month"}>{"1 month"}</option>
        <option value={"year"}>{"1 year"}</option>
        <option value={"all"}>{"all"}</option>
      </select>

      <select
        value={intervalRef.current}
        onChange={(e) => {
          const value: number = parseFloat(e.target.value);
          setInterval(value);
        }}
      >
        <option value={182.625}>{"6 months"}</option>
        <option value={7}>{"1 week"}</option>
        <option value={1}>{"1 day"}</option>
        <option value={0.04166666667}>{"1 hour"}</option>
        <option value={0.0006944444444}>{"1 min"}</option>
      </select>
      <ToggleButtonNotEmpty
        candleSelected={candleSelected}
        setCandleSelected={setCandleSelected}
      />
      {!candleSelected ? (
        <div
          style={{ width: 1200, backgroundColor: "#FFFFFF", margin: "35px" }}
        >
          <LineChart chartData={chartData} />
        </div>
      ) : (
        <CandleStickChart dataset={candleData} />
      )}
    </div>
  );
}

export default Graphs;
