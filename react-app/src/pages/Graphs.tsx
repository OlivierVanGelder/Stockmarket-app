import React, { useState, useEffect, useRef } from "react";
import "../App.css";
import LineChart from "../components/LineChart";
import CandleStickChart from "../components/CandleStickChart";
import CandleItem from "../CandleItem";
import ToggleButtonNotEmpty from "../components/ToggleButton";

async function fetchStockData(
  ticker: string,
  interval: number = 1,
  startDay: number,
  endDay: number
): Promise<any> {
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

async function fetchCandleStockData(
  ticker: string,
  interval: number = 1,
  startDay: number,
  endDay: number
): Promise<CandleItem[]> {
  try {
    const response = await fetch(
      `https://localhost:42069/candlestock?ticker=${ticker}&interval=${interval}&start=${startDay}&end=${endDay}`
    ).then((res) => res.json());
    return response;
  } catch (error) {
    console.error(`Error fetching data for ${ticker}:`, error);
    return [];
  }
}

function extractData(
  stockAmount: number,
  interval: number = 1,
  startDay: number
) {
  const labels: string[] = [];
  const baseDate = new Date("1980-01-01T12:00:00");

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
  const tickerRef = useRef(ticker);
  const [candleData, setCandleData] = useState<CandleDataItem[]>([]);
  const [interval, setInterval] = useState<number>(1 / 24);
  const [startDay, setStartDay] = useState<number>(15093);
  const [endDay, setEndDay] = useState<number>(15100);
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
    const newSocket = new WebSocket(`wss://localhost:42069/stockWS`);

    newSocket.onopen = () => {
      console.log("Connected to websocket");
      if (candleSelected) {
        newSocket.send(
          JSON.stringify(`${ticker}-${interval}-${startDay}-${endDay}-candle`)
        );
      } else if (!candleSelected) {
        newSocket.send(
          JSON.stringify(`${ticker}-${interval}-${startDay}-${endDay}`)
        );
      }
    };

    newSocket.onmessage = (event) => {
      const newData = JSON.parse(event.data);
      console.log("Received data:", newData);
      const tickerValue = tickerRef.current;

      if (candleSelected) {
        ApplyCandleData(
          tickerValue,
          interval,
          startDay,
          setCandleData,
          newData
        );
      } else {
        applyLineData(
          tickerValue,
          interval,
          startDay,
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
  }, [interval, startDay, endDay, candleSelected, ticker]);

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
