// pages/admin/Dashboard.jsx
import React, { useState, useEffect } from 'react';
import { 
  Grid, 
  Paper, 
  Typography, 
  Box,
  Card,
  CardContent
} from '@mui/material';
import { 
  BarChart, 
  Bar, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  Legend, 
  ResponsiveContainer,
  PieChart, 
  Pie, 
  Cell
} from 'recharts';
import apiService from '../../services/apiService';

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042'];

const AdminDashboard = () => {
  const [stats, setStats] = useState({
    bookings: [],
    revenue: 0,
    occupancyRate: 0,
    roomTypes: [],
    reviews: []
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // In a real app, you would fetch this from your API
        const mockData = {
          bookings: [
            { month: 'Jan', bookings: 12 },
            { month: 'Feb', bookings: 19 },
            { month: 'Mar', bookings: 15 },
            { month: 'Apr', bookings: 22 },
            { month: 'May', bookings: 18 },
            { month: 'Jun', bookings: 25 },
          ],
          revenue: 12500,
          occupancyRate: 78,
          roomTypes: [
            { name: 'Standard', value: 45 },
            { name: 'Deluxe', value: 30 },
            { name: 'Suite', value: 15 },
            { name: 'Business', value: 10 },
          ],
          reviews: [
            { rating: 5, count: 42 },
            { rating: 4, count: 35 },
            { rating: 3, count: 12 },
            { rating: 2, count: 6 },
            { rating: 1, count: 5 },
          ]
        };
        setStats(mockData);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching stats:', error);
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) return <Typography>Loading dashboard...</Typography>;

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>Admin Dashboard</Typography>
      
      <Grid container spacing={3}>
        {/* Summary Cards */}
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="text.secondary">Total Revenue</Typography>
              <Typography variant="h4">${stats.revenue.toLocaleString()}</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="text.secondary">Occupancy Rate</Typography>
              <Typography variant="h4">{stats.occupancyRate}%</Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="text.secondary">Total Bookings</Typography>
              <Typography variant="h4">
                {stats.bookings.reduce((sum, month) => sum + month.bookings, 0)}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Typography color="text.secondary">Pending Reviews</Typography>
              <Typography variant="h4">8</Typography>
            </CardContent>
          </Card>
        </Grid>

        {/* Charts */}
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>Monthly Bookings</Typography>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={stats.bookings}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="bookings" fill="#8884d8" />
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>Room Type Distribution</Typography>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={stats.roomTypes}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  outerRadius={80}
                  fill="#8884d8"
                  dataKey="value"
                  label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                >
                  {stats.roomTypes.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>

        <Grid item xs={12}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>Review Ratings</Typography>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={stats.reviews}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="rating" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="count" fill="#82ca9d" />
              </BarChart>
            </ResponsiveContainer>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default AdminDashboard;