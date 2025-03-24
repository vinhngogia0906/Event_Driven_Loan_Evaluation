import React, { useEffect, useState } from 'react';
import axios from 'axios';
import {
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    Typography,
    CircularProgress,
    Box,
    AppBar,
    Toolbar,
    Button,
} from '@mui/material';
import environment from '../environment';
import logo from '../assets/logo.png';
import './Dashboard.css';
import { useNavigate } from 'react-router-dom';

interface LoanApplication {
  id: string;
  name: string;
  loanLimit: number;
  purpose: string;
  customerId: string;
  approved: boolean;
  cancelled: boolean;
}

const Dashboard: React.FC = () => {
  const [loans, setLoans] = useState<LoanApplication[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate(); 

  const fetchLoans = async () => {
    try {
      const response = await axios.get(
        `${environment.backendUrl}/getLoanApplications`,
        {
          headers: {
            accept: 'text/plain',
          },
        }
      );
      setLoans(response.data);
    } catch (error) {
      setError('Failed to fetch loan applications. Please try again later.');
      console.error('Error fetching loan applications:', error);
    } finally {
      setLoading(false);
    }
  };
  useEffect(() => {
    fetchLoans();
  }, []);

  const handleLogout = () => {
    navigate('/');
  };

  const handleChangeApproval = async (applicationId: string, approvalStatus: boolean) => {
    try {
      await axios.put(
        `${environment.backendUrl}/updateLoanApplicationApproval`,
        null,
        {
          params: {
            applicationId,
            approvalStatus,
          },
          headers: {
            accept: 'text/plain',
          },
        }
      );

      // Refresh the loan applications after updating
      fetchLoans();
    } catch (error) {
      console.error('Error updating loan application approval:', error);
      setError('Failed to update loan application approval. Please try again later.');
    }
  };

  return (
    <Box>
        <AppBar position="static">
            <Toolbar>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                VinNance Credit Admin
            </Typography>
            <Button color="inherit" onClick={handleLogout}>
                Logout
            </Button>
            </Toolbar>
        </AppBar>
        <Box
        display="flex"
        flexDirection="column"
        alignItems="center"
        justifyContent="center"
        minHeight="80vh"
        minWidth='90vw'
        >
        <div className='logo'>
            <img src={logo} alt="Company Logo"/>
        </div>
        <Typography variant="h4" gutterBottom>
            Manage Loan Applications
        </Typography>
        {loading ? (
            <CircularProgress />
        ) : error ? (
            <Typography color="error">{error}</Typography>
        ) : loans.length === 0 ? (
            <Typography variant="body1">No loan applications found.</Typography>
        ) : (
            <TableContainer component={Paper} sx={{ 
              width: '100%',
              maxWidth: '95vw',
              mx: 'auto',     
              overflow: 'auto'
            }}>
            <Table sx={{ 
                      minWidth: 800,   // Minimum width before scrolling
                      tableLayout: 'auto' // Auto-layout or 'fixed' for equal columns
                    }}
                size="medium"
            >
                <TableHead>
                <TableRow>
                    <TableCell>ID</TableCell>
                    <TableCell>Name</TableCell>
                    <TableCell>Loan Limit</TableCell>
                    <TableCell>Purpose</TableCell>
                    <TableCell>Customer ID</TableCell>
                    <TableCell>Approved</TableCell>
                    <TableCell>Cancelled</TableCell>
                    <TableCell>Actions</TableCell>
                </TableRow>
                </TableHead>
                <TableBody>
                {loans.map((loan) => (
                    <TableRow key={loan.id}>
                    <TableCell>{loan.id}</TableCell>
                    <TableCell>{loan.name}</TableCell>
                    <TableCell>{loan.loanLimit}</TableCell>
                    <TableCell>{loan.purpose}</TableCell>
                    <TableCell>{loan.customerId}</TableCell>
                    <TableCell>{loan.approved ? 'Yes' : 'No'}</TableCell>
                    <TableCell>{loan.cancelled ? 'Yes' : 'No'}</TableCell>
                    <TableCell>
                      {loan.approved ? (
                        <Button
                          variant="contained"
                          color="error"
                          onClick={() => handleChangeApproval(loan.id, false)}
                          sx={{ margin: '4px' }}
                        >
                          Disapprove
                        </Button>
                      ) : (
                        <Button
                          variant="contained"
                          color="success"
                          onClick={() => handleChangeApproval(loan.id, true)}
                          sx={{ margin: '4px' }}
                        >
                          Approve
                        </Button>
                      )}
                    </TableCell>
                    </TableRow>
                ))}
                </TableBody>
            </Table>
            </TableContainer>
        )}
        </Box>
    </Box>
  );
};

export default Dashboard;